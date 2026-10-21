using MediCore.Application.Common.Exceptions;
using MediCore.Application.Common.Interfaces;
using MediCore.Application.DTOs.AccessAuthorization;
using MediCore.Application.DTOs.OTP;
using MediCore.Application.Interfaces;
using MediCore.Application.Interfaces.IServices;
using MediCore.Domain.Common;
using MediCore.Domain.Entities;
using MediCore.Domain.Enums;
using MediCore.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Services
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
            var (requesterUserId, requesterRole) = await GetApprovedRequesterAsync(cancellationToken);
            var patient = await _unitOfWork.Patients.GetByIdAsync(patientId, cancellationToken)
                ?? throw new NotFoundException(nameof(Patient), patientId);

            var activeAccess = await _unitOfWork.AccessAuthorizations.GetActiveAuthorizationAsync(patientId, requesterUserId, cancellationToken);
            if (activeAccess is not null)
                throw new BusinessRuleViolationException("You already have a pending access request for this patient, Ask the patient for the OTP that was already sent.");

            var accessRequest = new AccessAuthorization(patientId, requesterUserId, requesterRole, dto.Scope, dto.Purpose, dto.RequesteDurationMinutes);

            await _unitOfWork.AccessAuthorizations.AddAsync(accessRequest, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var patientEmail = await _identityService.GetEmailByUserIdAsync(patient.UserId, cancellationToken);
            var requestDto = new RequestOtpDto { Email = patientEmail, Purpose = OtpPurpose.AccessAuthorization, Channel = NotificationChannel.Email };
            var result = await _otpService.GenerateAndSendOtpAsync(requestDto, cancellationToken);

            if (!result.Success)
                throw new BusinessRuleViolationException(result.Message ?? "Failed to send OTP to the patient.");
            // OTP is temporary while production
            return ToDto(accessRequest, result.Otp);


        }

        public async Task<AccessRequestDto> GetRequestByIdAsync(Guid requestId, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.UserId ?? throw new AuthenticationRequiredException();
            var accessRequest = await _unitOfWork.AccessAuthorizations.GetByIdAsync(requestId, cancellationToken)
                ?? throw new NotFoundException(nameof(AccessAuthorization), requestId);

            var patient = await _unitOfWork.Patients.GetByUserIdAsync(currentUserId, cancellationToken);
            bool isPatientOwner = (patient != null && accessRequest.PatientId == patient.Id);
            bool isGranteeDoctor = (accessRequest.GranteeUserId == currentUserId);
            if (!isPatientOwner && !isGranteeDoctor && (_currentUserService.Role != AppRoles.SuperAdmin))
                throw new ForbiddenAccessException("you don't have permission to view this access request.");
            return ToDto(accessRequest);
        }

        public async Task<AccessRequestDto> VerifyAccessOtpAsync(Guid accessRequestId, VerifyAccessOtpDto dto, CancellationToken cancellationToken = default)
        {
            var (requesterUserId, requesterRole) = await GetApprovedRequesterAsync(cancellationToken);
            var accessRequest = await _unitOfWork.AccessAuthorizations.GetByIdAsync(accessRequestId, cancellationToken)
                ?? throw new NotFoundException(nameof(AccessAuthorization), accessRequestId);

            if (accessRequest.GranteeUserId != requesterUserId)
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


        public async Task<IEnumerable<AccessRequestDto>> GetPatientRequestsAsync(Guid? id, bool latestOnly = false, CancellationToken cancellationToken = default)
        {
            var patientId = await ResolvePatientIdAsync(id, cancellationToken);
            var requests = await _unitOfWork.AccessAuthorizations.GetByPatientAsync(patientId, cancellationToken);
            if (latestOnly)
            {
                var latestRequest = requests.FirstOrDefault();
                if (latestRequest is null)
                    return Enumerable.Empty<AccessRequestDto>();
                return new[] { ToDto(latestRequest) };
            }
            return requests.Select(r => ToDto(r));
        }

        public async Task<IEnumerable<AccessRequestDto>> GetRequestsByRequesterAsync(Guid? requesterId, bool latestOnly = false, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.UserId ?? throw new AuthenticationRequiredException();
            Guid targetUserId;

            if (!requesterId.HasValue || requesterId.Value == Guid.Empty)
            {
                targetUserId = currentUserId;
            }
            else
            {
                targetUserId = await GetUserIdFromRequesterIdAsync(requesterId.Value, cancellationToken);

                var isAdmin = _currentUserService.Role == AppRoles.SuperAdmin ||
                              _currentUserService.Role == AppRoles.HospitalAdmin ||
                              _currentUserService.Role == AppRoles.LabAdmin ||
                              _currentUserService.Role == AppRoles.PharmacyAdmin;

                if (targetUserId != currentUserId && !isAdmin)
                {
                    throw new ForbiddenAccessException("You are not authorized to view requests for another requester.");
                }
            }

            var requests = await _unitOfWork.AccessAuthorizations.GetByRequesterAsync(targetUserId, cancellationToken);

            if (latestOnly)
            {
                var latestRequest = requests.OrderByDescending(r => r.CreatedAt).FirstOrDefault();
                if (latestRequest is null)
                    return Enumerable.Empty<AccessRequestDto>();

                return new[] { ToDto(latestRequest) };
            }

            return requests.Select(r => ToDto(r));
        }

        public async Task RejectAccessRequestAsync(Guid requestId, CancellationToken cancellationToken = default)
        {
            var patient = await GetCurrentPatientAsync(cancellationToken);
            var accessRequest = await _unitOfWork.AccessAuthorizations.GetByIdAsync(requestId, cancellationToken);
            if (accessRequest is null) throw new NotFoundException($"Access request with ID: '{requestId}' Not found.");
            if (accessRequest.PatientId != patient.Id)
                throw new ForbiddenAccessException("You don't have permission to reject this access request.");

            accessRequest.Reject();

            _unitOfWork.AccessAuthorizations.Update(accessRequest);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }

        public async Task ApproveAccessRequestAsync(Guid requestId, CancellationToken cancellationToken = default)
        {
            var patient = await GetCurrentPatientAsync(cancellationToken);
            var accessRequest = await _unitOfWork.AccessAuthorizations.GetByIdAsync(requestId, cancellationToken)
                ?? throw new NotFoundException(nameof(AccessAuthorization), requestId);
            if (accessRequest.PatientId != patient.Id)
                throw new ForbiddenAccessException("You don't have permissioon to approve this access request.");

            accessRequest.Approve();

            _unitOfWork.AccessAuthorizations.Update(accessRequest);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }

        public async Task RevokeAccessRequestAsync(Guid accessRequestId, CancellationToken cancellationToken = default)
        {
            var patient = await GetCurrentPatientAsync(cancellationToken);

            var accessRequest = await _unitOfWork.AccessAuthorizations.GetByIdAsync(accessRequestId, cancellationToken)
                ?? throw new NotFoundException(nameof(AccessAuthorization), accessRequestId);

            if (accessRequest.PatientId != patient.Id)
                throw new ForbiddenAccessException("You do not have permission to revoke this access authorization.");

            accessRequest.Revoke();

            _unitOfWork.AccessAuthorizations.Update(accessRequest);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // ----- Helpers

        private async Task<(Guid userId, string Role)> GetApprovedRequesterAsync(CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId ?? throw new AuthenticationRequiredException();

            var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(currentUserId, cancellationToken);
            if (doctor is not null)
            {
                if (doctor.Status != Domain.Enums.DoctorStatus.Approved)
                    throw new ForbiddenAccessException("Only an approved doctor can perform this action.");
                return (doctor.UserId, AppRoles.Doctor);
            }

            var pharmacist = await _unitOfWork.Pharmacists.GetByUserIdAsync(currentUserId, cancellationToken);
            if (pharmacist is not null)
            {
                if (pharmacist.Status != Domain.Enums.PharmacistStatus.Approved)
                    throw new ForbiddenAccessException("Only an approved pharmacist can perform this action.");
                return (pharmacist.UserId, AppRoles.Pharmacist);
            }
            throw new ForbiddenAccessException("Only an approved doctor or pharmacist can perform this action.");

        }

        private async Task<Patient> GetCurrentPatientAsync(CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId ?? throw new AuthenticationRequiredException();

            var patient = await _unitOfWork.Patients.GetByUserIdAsync(currentUserId, cancellationToken)
                ?? throw new ForbiddenAccessException("Only a registered patient can perform this action.");

            return patient;
        }

        

        static AccessRequestDto ToDto(AccessAuthorization access, string? rawOtpCode = null) => new()
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

        async Task<Guid> ResolvePatientIdAsync(Guid? patientId, CancellationToken cancellationToken)
        {
            if (patientId.HasValue && patientId.Value != Guid.Empty)
                return patientId.Value;
            var currentUserId = RequireCurrentUserId();
            var role = _currentUserService.Role;

            if (string.Equals(role, AppRoles.Patient, StringComparison.OrdinalIgnoreCase))
            {
                var patient = await _unitOfWork.Patients.GetByUserIdAsync(currentUserId, cancellationToken)
                    ?? throw new NotFoundException($"Patient profile not found for user ID: '{currentUserId}'.");
                return patient.Id;
            }

            throw new ArgumentException("PatientId query parameter is required for non-patient callers.");

        }
        Guid RequireCurrentUserId()
            => _currentUserService.UserId ?? throw new AuthenticationRequiredException();

        private async Task<Guid> GetUserIdFromRequesterIdAsync(Guid requesterId, CancellationToken cancellationToken)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(requesterId, cancellationToken);
            if (doctor is not null) return doctor.UserId;

            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(requesterId, cancellationToken);
            if (pharmacist is not null) return pharmacist.UserId;

            var labTech = await _unitOfWork.LabTechnicians.GetByIdAsync(requesterId, cancellationToken);
            if (labTech is not null) return labTech.UserId;

            throw new NotFoundException("Requester", requesterId);
        }

    }
}
