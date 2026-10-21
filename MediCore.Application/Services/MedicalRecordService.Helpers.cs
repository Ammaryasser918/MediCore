using MediCore.Application.Common.Exceptions;
using MediCore.Application.DTOs.MedicalRecord;
using MediCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediCore.Domain.Enums;
using MediCore.Application.DTOs.MedicalFiles;

namespace MediCore.Application.Services
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


        MedicalRecordDto ToDto(MedicalRecord record) => new()
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
            Files = record.MedicalFiles.Select(f => MedicalFileMapper.ToDto(f, _fileStorageService)).ToList(),
            CreatedAt = record.CreatedAt,
            UpdatedAt = record.UpdatedAt
        };

        async Task EnsureCanAccessPatientRecordAsync(Patient patient, CancellationToken cancellationToken = default)
        {
            var currentUserId = RequireCurrentUserId();
            
            await _accessValicationService.EnsureAccessAsync(patient, currentUserId, AccessScope.FullMedicalRecord, cancellationToken);
        }

        async Task EnsureCanAccessRecordAsync(MedicalRecord record, CancellationToken cancellationToken)
        {
            var currentUserId = RequireCurrentUserId();

            var owningPatient = await _unitOfWork.Patients.GetByIdAsync(record.PatientId, cancellationToken);
            if (owningPatient is not null && owningPatient.UserId == currentUserId) return;

            // The doctor who authored this record can always view it.
            var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(currentUserId, cancellationToken);
            if (doctor is not null && doctor.Id == record.DoctorId)
                return;

            await _accessValicationService.EnsureAccessAsync(record.Patient, currentUserId, AccessScope.FullMedicalRecord, cancellationToken);
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

        

    }
}
