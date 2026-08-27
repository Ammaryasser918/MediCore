using PulseID.Application.Common.Exceptions;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.MedicalRecord;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Entities;
using PulseID.Domain.Enums;
using PulseID.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Services
{
    public partial class MedicalRecordService : IMedicalRecordService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly ICurrentUserService _currentUserService;
        readonly IAccessValidationService _accessValicationService;
        
        public MedicalRecordService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IAccessValidationService accessValicationService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _accessValicationService = accessValicationService;
        }


        public async Task<MedicalRecordDto> CreateAsync(CreateMedicalRecordDto dto, CancellationToken cancellationToken = default)
        {
            var doctor = await GetRequestingApprovedDoctorAsync(cancellationToken);
            var patient = await _unitOfWork.Patients.GetByIdAsync(dto.PatientId, cancellationToken)
                ?? throw new NotFoundException(nameof(Patient), dto.PatientId);


            await EnsureCanAccessPatientRecordAsync(dto.PatientId, cancellationToken);


            if (doctor.Department is null)
                throw new BusinessRuleViolationException(
                    $"Doctor {doctor.Id} has an invalid DepartmentId ({doctor.DepartmentId}) — it doesn't match any existing Department. Fix the doctor's department assignment before retrying.");

            Guid currentUserId = RequireCurrentUserId();


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


            bool isPatient = (patient.UserId == currentUserId);

            if (!isPatient)
            {
                await EnsureCanAccessPatientRecordAsync(patientId, cancellationToken);
            }

            var records = await _unitOfWork.MedicalRecords.GetByPatientAsync(patientId, cancellationToken);

            if (LatestVersionOnly)
                records = records.Where(r => r.Status == MedicalRecordStatus.Active).ToList();
            
            return records.OrderByDescending(r => r.VisitDate).Select(ToListItemDto).ToList();

        }
        

        public async Task<IReadOnlyList<MedicalRecordVersionDto>> GetVersionHistoryAsync(Guid anyVersionId, CancellationToken cancellationToken = default)
        {
            var reference = await _unitOfWork.MedicalRecords.GetByIdAsync(anyVersionId, cancellationToken) ?? throw new NotFoundException(nameof(MedicalRecord), anyVersionId);
            await EnsureCanAccessRecordAsync(reference, cancellationToken);

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

            await _accessValicationService.EnsureAccessAsync(currentVersion.PatientId, doctor.UserId, AccessScope.FullMedicalRecord, cancellationToken);

            if (doctor.Department is null)
                throw new BusinessRuleViolationException(
                    $"Doctor {doctor.Id} has an invalid DepartmentId ({doctor.DepartmentId}) — it doesn't match any existing Department. Fix the doctor's department assignment before retrying.");

            var newVersion = currentVersion.CreateNewVersion(doctor.Id, doctor.Department.HospitalId, dto.Diagnosis, dto.Symptoms, dto.Notes);

            _unitOfWork.MedicalRecords.Update(currentVersion);
            await _unitOfWork.MedicalRecords.AddAsync(newVersion, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToDto(newVersion);

        }






    }
}
