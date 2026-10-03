using PulseID.Application.Common.Exceptions;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.Medications;
using PulseID.Application.DTOs.PatientMedications;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Common;
using PulseID.Domain.Entities;
using PulseID.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Services
{
    public class PatientMedicationService : IPatientMedicationService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IAccessValidationService _accessValidationService;
        readonly ICurrentUserService _currentUserService;

        public PatientMedicationService(IUnitOfWork unitOfWork, IAccessValidationService accessValidationService, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _accessValidationService = accessValidationService;
            _currentUserService = currentUserService;
        }


        Guid RequireCurrentUserId()
            => _currentUserService.UserId ?? throw new AuthenticationRequiredException();

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

        async Task<Patient> GetPatientWithAccessAsync(Guid patientId, CancellationToken cancellationToken)
        {
            var patient = await _unitOfWork.Patients.GetWithFullProfileAsync(patientId, cancellationToken)
                ?? throw new NotFoundException(nameof(Patient), patientId);

            var currentuserId = RequireCurrentUserId();

            if (patient.UserId != currentuserId)
            {
                await _accessValidationService.EnsureAccessAsync(patient, currentuserId, Domain.Enums.AccessScope.FullMedicalRecord, cancellationToken);
            }
            return patient;
        }

        static PatientMedicationDto ToMedicationDto(PatientMedication pm) => new()
        {
            Id = pm.Id,
            PatientId = pm.PatientId,
            MedicationId = pm.MedicationId,
            MedicationName = pm.Medication.CommercialNameEn,
            MedicationNameAr = pm.Medication.CommercialNameAr,
            ScientificName = pm.Medication.ScientificName,
            Dosage = pm.Dosage,
            Frequency = pm.Frequency,
            StartDate = pm.StartDate,
            EndDate = pm.EndDate,
            IsActive = pm.IsActive
        };


        public async Task<IReadOnlyList<PatientMedicationDto>> GetPatientMedicationsAsync(Guid? patientId, bool activeOnly, CancellationToken cancellationToken = default)
        {
            var resolvedPatientId = await ResolvePatientIdAsync(patientId, cancellationToken);
            var patient = await GetPatientWithAccessAsync(resolvedPatientId, cancellationToken);

            return patient.PatientMedications
                .Where(pm => !activeOnly || pm.IsActive)
                .OrderByDescending(pm => pm.IsActive)
                .ThenByDescending(pm => pm.StartDate ?? pm.CreatedAt)
                .Select(ToMedicationDto)
                .ToList();
        }

        public async Task<PatientMedicationDto> AddMedicationAsync(Guid? patientId, AddPatientMedicationDto dto, CancellationToken cancellationToken = default)
        {
            var resolvedPatientId = await ResolvePatientIdAsync(patientId, cancellationToken);
            var patient = await GetPatientWithAccessAsync(resolvedPatientId, cancellationToken);

            var medication = await _unitOfWork.Medications.GetByIdAsync(dto.MedicationId, cancellationToken)
                ?? throw new NotFoundException(nameof(Medication), dto.MedicationId);

            var newMedication = patient.AddMedication(dto.MedicationId, dto.Dosage, dto.Frequency, dto.StartDate, dto.EndDate);

            await _unitOfWork.TrackAsAddedAsync(newMedication, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new PatientMedicationDto
            {
                Id = newMedication.Id,
                PatientId = resolvedPatientId,
                MedicationId = medication.Id,
                MedicationName = medication.CommercialNameEn,
                MedicationNameAr = medication.CommercialNameAr,
                ScientificName = medication.ScientificName,
                Dosage = newMedication.Dosage,
                Frequency = newMedication.Frequency,
                StartDate = newMedication.StartDate,
                EndDate = newMedication.EndDate,
                IsActive = newMedication.IsActive
            };
        }

        public async Task StopMedicationAsync(Guid? patientId, Guid patientMedicationId, StopPatientMedicationDto dto, CancellationToken cancellationToken = default)
        {
            var resolvedPatientId = await ResolvePatientIdAsync(patientId, cancellationToken);
            var patient = await GetPatientWithAccessAsync(resolvedPatientId, cancellationToken);

            var existing = patient.PatientMedications.FirstOrDefault(pm => pm.Id == patientMedicationId)
                ?? throw new NotFoundException($"Patient {resolvedPatientId} doesn't have medication record {patientMedicationId} on file.");

            if (!existing.IsActive)
                throw new BusinessRuleViolationException("This medication is already stopped.");

            patient.StopMedication(patientMedicationId, dto?.EndDate);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }


    }
}
