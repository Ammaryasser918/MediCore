using PulseID.Application.Common.Exceptions;
using PulseID.Application.DTOs.MedicalRecord;
using PulseID.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PulseID.Domain.Enums;
using PulseID.Application.DTOs.MedicalFiles;

namespace PulseID.Application.Services
{
    public partial class MedicalRecordService
    {
        private async Task<Doctor> GetRequestingApprovedDoctorAsync(CancellationToken cancellationToken)
        {
            var currentUserId = RequireCurrentUserId();

            var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(currentUserId, cancellationToken)
                ?? throw new ForbiddenAccessException("Only a registered doctor can perform this action.");

            if (doctor.Status != Domain.Enums.DoctorStatus.Approved)
                throw new ForbiddenAccessException("Only an approved doctor can perform this action.");

            return doctor;


        }

        private Guid RequireCurrentUserId()
            => _currentUserService.UserId ?? throw new AuthenticationRequiredException();


        static MedicalRecordDto ToDto(MedicalRecord record) => new()
        {
            Id = record.Id,
            PatientId = record.PatientId,
            DoctorId = record.DoctorId,
            HospitalId = record.HospitalId,
            VisitDate = record.VisitDate,
            Diagnosis = record.Diagnosis,
            Symptoms = record.Symptoms,
            Notes = record.Notes,
            Status = record.Status,
            PrescriptionId = record.PrescriptionId,
            PreviousVersionId = record.PreviousVersionId,
            RootRecordId = record.RootRecordId,
            IsLatestVersion = record.Status == MedicalRecordStatus.Active,
            Files = record.MedicalFiles.Select(ToFileDto).ToList(),
            CreatedAt = record.CreatedAt,
            UpdatedAt = record.UpdatedAt
        };

        async Task EnsureCanAccessRecordAsync(MedicalRecord record, CancellationToken cancellationToken = default)
        {
            var currentUserId = RequireCurrentUserId();
            var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(currentUserId, cancellationToken);

            if (doctor is not null)
                return;

            await _accessValicationService.EnsureAccessAsync(record.PatientId, currentUserId, AccessScope.FullMedicalRecord, cancellationToken);
        }


        static MedicalRecordListItemDto ToListItemDto(MedicalRecord record) => new()
        {
            Id = record.Id,
            PatientId = record.PatientId,
            DoctorId = record.DoctorId,
            HospitalId = record.HospitalId,
            VisitDate = record.VisitDate,
            Diagnosis = record.Diagnosis,
            Status = record.Status,
            RootRecordId = record.RootRecordId,
            CreatedAt = record.CreatedAt
        };

        internal static MedicalFileDto ToFileDto(MedicalFile file) => new()
        {
            Id = file.Id,
            PatientId = file.PatientId,
            MedicalRecordId = file.MedicalRecordId,
            FileName = file.FileName,
            FilePath = file.FilePath,
            ContentType = file.ContentType,
            FileType = file.FileType,
            Description = file.Description,
            CreatedAt = file.CreatedAt
        };

    }
}
