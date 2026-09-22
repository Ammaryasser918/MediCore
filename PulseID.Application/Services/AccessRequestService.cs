using PulseID.Application.Common.Exceptions;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.AccessAuthorization;
using PulseID.Application.DTOs.OTP;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Entities;
using PulseID.Domain.Enums;
using PulseID.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Services
{
    public class AccessRequestService : IAccessRequestService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly ICurrentUserService _currentUserService;
        readonly IIdentityService _identityService;
        readonly IOTPService _otpService;

        public AccessRequestService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IIdentityService identityService,  IOTPService otpService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _identityService = identityService;
            _otpService = otpService;
        }

        public async Task<AccessRequestDto> RequestAccessAsync(Guid patientId, RequestPatientAccessDto dto, CancellationToken cancellationToken = default)
        {
            var doctor = await GetRequestingApprovedDoctorAsync(cancellationToken);
            var patient = await _unitOfWork.Patients.GetByIdAsync(patientId, cancellationToken)
                ?? throw new NotFoundException(nameof(Patient), patientId);

            var activeAccess = await _unitOfWork.AccessAuthorizations.GetActiveAuthorizationAsync(patientId, doctor.UserId, cancellationToken);
            if (activeAccess is not null)
                throw new BusinessRuleViolationException("You already have a pending access request for this patient, Ask the patient for the OTP that was already sent.");

            var accessRequest = new AccessAuthorization(patientId, doctor.UserId, "Doctor", dto.Scope, dto.Purpose, dto.RequesteDurationMinutes);

            await _unitOfWork.AccessAuthorizations.AddAsync(accessRequest, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var patientEmail = await _identityService.GetEmailByUserIdAsync(patient.UserId, cancellationToken);
            var requestDto = new RequestOtpDto { Email = patientEmail, Purpose = OtpPurpose.AccessAuthorization, Channel = OtpChannel.Email };
            var result = await _otpService.GenerateAndSendOtpAsync(requestDto, cancellationToken);

            if (!result.Success)
                throw new BusinessRuleViolationException(result.Message ?? "Failed to send OTP to the patient.");

            return ToDto(accessRequest, result.Otp);


        }

        public async Task<AccessRequestDto> VerifyAccessOtpAsync(Guid accessRequestId, VerifyAccessOtpDto dto, CancellationToken cancellationToken = default)
        {
            var doctor = await GetRequestingApprovedDoctorAsync(cancellationToken);
            var accessRequest = await _unitOfWork.AccessAuthorizations.GetByIdAsync(accessRequestId, cancellationToken)
                ?? throw new NotFoundException(nameof(AccessAuthorization), accessRequestId);

            if (accessRequest.GranteeUserId != doctor.UserId)
                throw new ForbiddenAccessException("This access request does not belong to you.");

            var patient = await _unitOfWork.Patients.GetByIdAsync(accessRequest.PatientId, cancellationToken)
                ?? throw new NotFoundException(nameof(Patient), accessRequest.PatientId);

            var patientEmail = await _identityService.GetEmailByUserIdAsync(patient.UserId, cancellationToken)
                ?? throw new BusinessRuleViolationException("Patient doesn't have a registered email.");

            var verifyDto = new VerifyOtpDto { Email = patientEmail, Otp = dto.Otp, Purpose = OtpPurpose.AccessAuthorization };
            var verification = await _otpService.VerifyOtpAsync(verifyDto, cancellationToken);

            if (!verification.Success)
                throw new BusinessRuleViolationException(verification.Message ?? "Invalid or expired OTP.");

            accessRequest.Approve();

            _unitOfWork.AccessAuthorizations.Update(accessRequest);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToDto(accessRequest, dto.Otp);

        }


        // ----- Helpers

        private async Task<Doctor> GetRequestingApprovedDoctorAsync(CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId ?? throw new AuthenticationRequiredException();

            var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(currentUserId, cancellationToken)
                ?? throw new ForbiddenAccessException("Only a registered doctor can perform this action.");

            if (doctor.Status != Domain.Enums.DoctorStatus.Approved)
                throw new ForbiddenAccessException("Only an approved doctor can perform this action.");
            return doctor;

        }

        static AccessRequestDto ToDto(AccessAuthorization access, string rawOtpCode) => new()
        {
            Id = access.Id,
            PatientId = access.PatientId,
            GranteeUserId = access.GranteeUserId,
            Purpose = access.Purpose,
            Scope = access.Scope,
            Status = access.Status,
            RequestedDurationMinutes = access.RequestedDurationMinutes,
            RequestedAt = access.RequestedAt,
            RespondedAt = access.RespondedAt,
            ExpiresAt = access.ExpiresAt,
            Otp = rawOtpCode
        };


    }
}
