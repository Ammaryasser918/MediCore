using MediCore.Application.Common.Exceptions;
using MediCore.Application.Common.Interfaces;
using MediCore.Application.DTOs.MedicalRecord;
using MediCore.Application.Interfaces;
using MediCore.Application.Interfaces.IServices;
using MediCore.Domain.Common;
using MediCore.Domain.Entities;
using MediCore.Domain.Enums;
using MediCore.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Services
{
    public partial class MedicalRecordService : IMedicalRecordService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly ICurrentUserService _currentUserService;
        readonly IAccessValidationService _accessValicationService;
        readonly IFileStorageService _fileStorageService;

        public MedicalRecordService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IAccessValidationService accessValicationService, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _accessValicationService = accessValicationService;
            _fileStorageService = fileStorageService;
        }


        public async Task<MedicalRecordDto> CreateAsync(CreateMedicalRecordDto dto, CancellationToken cancellationToken = default)
        {
            var doctor = await GetRequestingApprovedDoctorAsync(cancellationToken);
            var patient = await _unitOfWork.Patients.GetByIdAsync(dto.PatientId, cancellationToken)
                ?? throw new NotFoundException(nameof(Patient), dto.PatientId);


            await EnsureCanAccessPatientRecordAsync(patient, cancellationToken);


            if (doctor.Department is null)
                throw new BusinessRuleViolationException(
                    $"Doctor {doctor.Id} has an invalid DepartmentId ({doctor.DepartmentId}) — it doesn't match any existing Department. Fix the doctor's department assignment before retrying.");

            //Guid currentUserId = RequireCurrentUserId();


            var record = new MedicalRecord(
                patient.Id,
                doctor.Id,
                doctor.Department.HospitalId,
                dto.VisitDate,
                dto.Diagnosis,
                dto.Symptoms,
                dto.Notes
                );

            await _unitOfWork.MedicalRecords.AddAsync(record, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToDto(record);
            
        }

        public async Task<MedicalRecordDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var record = await _unitOfWork.MedicalRecords.GetWithDetailsAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(MedicalRecord), id);

            await EnsureCanAccessRecordAsync(record, cancellationToken);

            return ToDto(record);

        }

        public async Task<IReadOnlyList<MedicalRecordListItemDto>> GetByPatientAsync(Guid patientId, bool LatestVersionOnly = true, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(patientId, cancellationToken) ?? throw new NotFoundException(nameof(Patient), patientId);

            var currentUserId = RequireCurrentUserId();


            await EnsureCanAccessPatientRecordAsync(patient, cancellationToken);

            var records = await _unitOfWork.MedicalRecords.GetByPatientAsync(patientId, cancellationToken);

            if (LatestVersionOnly)
                records = records.Where(r => r.Status == MedicalRecordStatus.Active).ToList();
            
            return records.OrderByDescending(r => r.VisitDate).Select(ToListItemDto).ToList();

        }
        
        public async Task<IReadOnlyList<MedicalRecordListItemDto>> GetByDoctorAsync(Guid? doctorId, bool LatestVersionOnly = true, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.UserId ?? throw new AuthenticationRequiredException();

            Guid targetDoctorId = Guid.Empty;
            if (!doctorId.HasValue || doctorId.Value == Guid.Empty)
            {
                var currentDoctor = await _unitOfWork.Doctors.GetByUserIdAsync(currentUserId, cancellationToken)
                    ?? throw new NotFoundException("Doctor profile not found for current user.");
                targetDoctorId = currentDoctor.Id;
            }
            else
            {
                var doctor = await _unitOfWork.Doctors.GetByIdAsync(doctorId.Value, cancellationToken)
                    ?? throw new NotFoundException(nameof(Doctor), doctorId.Value);
                var isAdmin = (_currentUserService.Role == AppRoles.SuperAdmin || _currentUserService.Role == AppRoles.HospitalAdmin);
                if (doctor.UserId != currentUserId && !isAdmin)
                    throw new ForbiddenAccessException("You do not have permission to access records for this doctor.");
                targetDoctorId = doctor.Id;
            }

            var records = await _unitOfWork.MedicalRecords.GetByDoctorAsync(targetDoctorId, cancellationToken);
            return records.Select(ToListItemDto).ToList();
        }

        public async Task<IReadOnlyList<MedicalRecordVersionDto>> GetVersionHistoryAsync(Guid anyVersionId, CancellationToken cancellationToken = default)
        {
            var reference = await _unitOfWork.MedicalRecords.GetByIdAsync(anyVersionId, cancellationToken) ?? throw new NotFoundException(nameof(MedicalRecord), anyVersionId);
            await EnsureCanAccessPatientRecordAsync(reference.Patient, cancellationToken);

            var latest = await _unitOfWork.MedicalRecords.GetLatestVersionAsync(reference.RootRecordId, cancellationToken) ?? reference;

            var chain = new List<MedicalRecord>();
            MedicalRecord? current = latest;
            while (current is not null)
            {
                chain.Add(current);
                current = current.PreviousVersionId.HasValue ? await _unitOfWork.MedicalRecords.GetByIdAsync(current.PreviousVersionId.Value, cancellationToken) : null;
            }

            chain.Reverse();

            return chain.Select(record => new MedicalRecordVersionDto
            {
                Id = record.Id,
                PreviousVersionId = record.PreviousVersionId,
                VisitDate = record.VisitDate,
                Diagnosis = record.Diagnosis,
                Symptoms = record.Symptoms,
                Notes = record.Notes,
                Status = record.Status,
                IsLatestVersion = (record.Status == MedicalRecordStatus.Active),
                CreatedAt = record.CreatedAt,
                UpdatedAt = record.UpdatedAt
            }).ToList();

        }

        public async Task<MedicalRecordDto> CreateNewVersionAsync(Guid currentVersionId, CreateMedicalRecordVersionDto dto, CancellationToken cancellationToken = default)
        {

            var doctor = await GetRequestingApprovedDoctorAsync(cancellationToken);

            var currentVersion = await _unitOfWork.MedicalRecords.GetByIdAsync(currentVersionId, cancellationToken) 
                ?? throw new NotFoundException(nameof(MedicalRecord), currentVersionId);
            var patient = await _unitOfWork.Patients.GetByIdAsync(currentVersion.PatientId, cancellationToken)
                ?? throw new NotFoundException(nameof(Patient), currentVersion.PatientId);
            await _accessValicationService.EnsureAccessAsync(patient, doctor.UserId, AccessScope.FullMedicalRecord, cancellationToken);

            if (doctor.Department is null)
                throw new BusinessRuleViolationException(
                    $"Doctor {doctor.Id} has an invalid DepartmentId ({doctor.DepartmentId}) — it doesn't match any existing Department. Fix the doctor's department assignment before retrying.");

            var newVersion = currentVersion.CreateNewVersion(doctor.Id, doctor.Department.HospitalId, dto.Diagnosis, dto.Symptoms, dto.Notes);

            _unitOfWork.MedicalRecords.Update(currentVersion);
            await _unitOfWork.MedicalRecords.AddAsync(newVersion, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToDto(newVersion);

        }


        public async Task<IReadOnlyList<MedicalRecordDto>> GetDeletedMedicalRecordsAsync(CancellationToken cancellationToken = default)
        {
            var deletedMedicalRecord = await _unitOfWork.MedicalRecords.GetDeletedAsync(cancellationToken);
            var medicalRecordDtos = new List<MedicalRecordDto>();
            foreach(var medicalRecord in deletedMedicalRecord)
            {
                medicalRecordDtos.Add(ToDto(medicalRecord));
            }
            return medicalRecordDtos;
        }

        public async Task RestoreMedicalRecordAsync(Guid medicalRecordId, CancellationToken cancellationToken = default)
        {
            var medicalRecord = await _unitOfWork.MedicalRecords.GetDeletedByIdAsync(medicalRecordId, cancellationToken);
            if (medicalRecord is null) throw new NotFoundException("Deleted medical record not found.");
            _unitOfWork.MedicalRecords.Restore(medicalRecord);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

    }
}
