using PulseID.Application.Common.Exceptions;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.MedicalFiles;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Entities;
using PulseID.Domain.Enums;
using PulseID.Domain.Exceptions;
using PulseID.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Services
{
    public class MedicalFileService : IMedicalFileService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IAccessValidationService _accessValidationService;
        readonly ICurrentUserService _currentUser;

        public MedicalFileService(IUnitOfWork unitOfWork, IAccessValidationService accessValidationService, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _accessValidationService = accessValidationService;
            _currentUser = currentUser;
        }


        Guid RequireCurrentUserId()
            => _currentUser.UserId ?? throw new AuthenticationRequiredException();


        public async Task<MedicalFileDto> AddFileAsync(Guid medicalRecordId, AddMedicalFileDto dto, CancellationToken cancellationToken = default)
        {
            var record = await _unitOfWork.MedicalRecords.GetByIdAsync(medicalRecordId, cancellationToken)
                ?? throw new NotFoundException(nameof(MedicalRecord), medicalRecordId);

            if (record.Status == MedicalRecordStatus.Superseded)
            {
                throw new BusinessRuleViolationException("Can't attach files to a supereded (old) version of a medical record. Attach to the latest version instead.");
            }


            var currentUserId = RequireCurrentUserId();

            await _accessValidationService.EnsureAccessAsync(record.PatientId, currentUserId, AccessScope.FullMedicalRecord, cancellationToken);

            var patient = await _unitOfWork.Patients.GetByIdAsync(record.PatientId, cancellationToken) ?? throw new NotFoundException(nameof(Patient), record.PatientId);

            var file = patient.AddMedicalFile(record.Id, dto.FileName, dto.FilePath, dto.ContentType, dto.FileType, dto.Description);

            await _unitOfWork.TrackAsAddedAsync(file, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return MedicalRecordService.ToFileDto(file);

        }

        public async Task<IReadOnlyList<MedicalFileDto>> GetByMedicalRecordAsync(Guid medicalRecordId, CancellationToken cancellationToken = default)
        {
            var record = await _unitOfWork.MedicalRecords.GetWithDetailsAsync(medicalRecordId, cancellationToken)
                ?? throw new NotFoundException(nameof(MedicalRecord), medicalRecordId);

            var currentUserId = RequireCurrentUserId();

            await _accessValidationService.EnsureAccessAsync(record.PatientId, currentUserId, AccessScope.FullMedicalRecord, cancellationToken);

            return record.MedicalFiles.Select(MedicalRecordService.ToFileDto).ToList();

        }


    }
}
