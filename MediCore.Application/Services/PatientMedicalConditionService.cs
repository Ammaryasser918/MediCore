using MediCore.Application.Common.Exceptions;
using MediCore.Application.Common.Interfaces;
using MediCore.Application.DTOs.Allergies;
using MediCore.Application.DTOs.ChronicDiseases;
using MediCore.Application.Interfaces;
using MediCore.Application.Interfaces.IServices;
using MediCore.Domain.Common;
using MediCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Services
{
    public class PatientMedicalConditionService : IPatientMedicalConditionService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IAccessValidationService _accessValidationService;
        readonly ICurrentUserService _currentUserService;

        public PatientMedicalConditionService(IUnitOfWork unitOfWork, IAccessValidationService accessValidationService, ICurrentUserService currentUserService)
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

        // Allergies
        static PatientAllergyDto ToAllergyDto(PatientAllergy pa) => new()
        {
            PatientId = pa.PatientId,
            AllergyId = pa.AllergyId,
            AllergyName = pa.Allergy.Name,
            AllergyDescription = pa.Allergy.Description,
            DiagnosedDate = pa.DiagnosedDate
        };


        public async Task<IReadOnlyList<PatientAllergyDto>> GetPatientAllergiesAsync(Guid? patientId, CancellationToken cancellationToken = default)
        {
            var resolvepatientId = await ResolvePatientIdAsync(patientId, cancellationToken);
            var patient = await GetPatientWithAccessAsync(resolvepatientId, cancellationToken);
            return patient.PatientAllergies.Select(ToAllergyDto).ToList();
        }

        public async Task<PatientAllergyDto> AddAllergyAsync(Guid? patientId, AddPatientAllergyDto dto, CancellationToken cancellationToken = default)
        {
            var resolvePatientId = await ResolvePatientIdAsync(patientId, cancellationToken);
            var patient = await GetPatientWithAccessAsync(resolvePatientId, cancellationToken);
            var allergy = await _unitOfWork.Allergies.GetByIdAsync(dto.AllergyId, cancellationToken)
                ?? throw new NotFoundException(nameof(Allergy), dto.AllergyId);

            var linkWithpatient = patient.AddAllergy(dto.AllergyId, dto.DiagnosedDate);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new PatientAllergyDto
            {
                PatientId = resolvePatientId,
                AllergyId = allergy.Id,
                AllergyName = allergy.Name,
                AllergyDescription = allergy.Description,
                DiagnosedDate = linkWithpatient.DiagnosedDate
            };


        }

        public async Task RemoveAllergyAsync(Guid? patientId, Guid allergyId,  CancellationToken cancellationToken = default)
        {
            var resolvedPatientId = await ResolvePatientIdAsync(patientId, cancellationToken);
            var patient = await GetPatientWithAccessAsync(resolvedPatientId, cancellationToken);

            if (patient.PatientAllergies.All(pa => pa.AllergyId != allergyId))
                throw new NotFoundException($"Paitent {resolvedPatientId} doesn't have allergy {allergyId} on file.");

            patient.RemoveAllergy(allergyId);
            await _unitOfWork.SaveChangesAsync( cancellationToken);

        }



        // Chronic Diseases

        static PatientChronicDiseaseDto ToChronicDiseaseDto(PatientChronicDisease pcd) => new()
        {
            PatientId = pcd.PatientId,
            ChronicDiseaseId = pcd.ChronicDiseaseId,
            ChronicDiseaseName = pcd.ChronicDisease.Name,
            ChronicDiseaseDescription = pcd.ChronicDisease.Description,
            DiagnosedDate = pcd.DiagnosedDate
        };


        public async Task<IReadOnlyList<PatientChronicDiseaseDto>> GetPatientChronicDiseasesAsync(Guid? patientId, CancellationToken cancellationToken = default)
        {
            var resolvedPatientId = await ResolvePatientIdAsync(patientId, cancellationToken);
            var patient = await GetPatientWithAccessAsync(resolvedPatientId, cancellationToken);
            return patient.PatientChronicDiseases.Select(ToChronicDiseaseDto).ToList();
        }

        public async Task<PatientChronicDiseaseDto> AddChronicDiseaseAsync(Guid? patientId, AddPateintChronicDiseaseDto dto,  CancellationToken cancellationToken = default)
        {
            var resolvedPatientId = await ResolvePatientIdAsync(patientId, cancellationToken);
            var patient = await GetPatientWithAccessAsync(resolvedPatientId, cancellationToken);

            var disease = await _unitOfWork.ChronicDiseases.GetByIdAsync(dto.ChronicDiseaseId, cancellationToken)
                ?? throw new NotFoundException(nameof(ChronicDisease), dto.ChronicDiseaseId);

            var linkWithPatient = patient.AddChronicDisease(dto.ChronicDiseaseId, dto.DiagnosedDate);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new PatientChronicDiseaseDto
            {
                PatientId = resolvedPatientId,
                ChronicDiseaseId = disease.Id,
                ChronicDiseaseName = disease.Name,
                ChronicDiseaseDescription = disease.Description,
                DiagnosedDate = linkWithPatient.DiagnosedDate
            };

        }

        public async Task RemoveChronicDiseasesAsync(Guid? patientId, Guid chronicDiseaseId, CancellationToken cancellationToken = default)
        {
            var resolvedPatientId = await ResolvePatientIdAsync(patientId, cancellationToken);
            var patient = await GetPatientWithAccessAsync(resolvedPatientId, cancellationToken);

            if (patient.PatientChronicDiseases.All(pcd => pcd.ChronicDiseaseId != chronicDiseaseId))
                throw new NotFoundException($"Patient {resolvedPatientId} doesn't have chronic disease {chronicDiseaseId} on files.");
            patient.RemoveChronicDisease(chronicDiseaseId);
            await _unitOfWork.SaveChangesAsync( cancellationToken);

        }


    }
}
