using PulseID.Application.Common.Exceptions;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.Patient;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Common;
using PulseID.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Services
{
    public class PatientService : IPatientService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IIdentityService _identityService;
        public PatientService(IUnitOfWork unitOfWork, IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }

        static PatientDto ToDto(Patient patient, string email)
            => new()
            {
                Id = patient.Id,
                Email = email,
                NationalId = patient.NationalId,
                FullName = patient.FullName,
                DateOfBirth = patient.DateOfBirth,
                Age = patient.GetAge(),
                Gender = patient.Gender,
                BloodType = patient.BloodType,
                PhoneNumber = patient.PhoneNumber,
                Address = patient.Address,
                CreatedAt = patient.CreatedAt,
                UpdatedAt = patient.UpdatedAt
            };





        public async Task<IReadOnlyList<PatientDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var patients = await _unitOfWork.Patients.GetAllAsync(cancellationToken);
            var patientDtos = new List<PatientDto>();
            foreach (var patient in patients)
            {
                var email = await _identityService.GetEmailByUserIdAsync(patient.UserId, cancellationToken);
                patientDtos.Add(ToDto(patient, email));
            }
            return patientDtos;
        }

        public async Task<PatientDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Patient), id);
            var email = await _identityService.GetEmailByUserIdAsync(patient.UserId, cancellationToken);
            return ToDto(patient, email);
        }
        public async Task<PatientDto> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Patients.GetByUserIdAsync(userId, cancellationToken) ?? throw new NotFoundException(nameof(Patient), userId);
            var email = await _identityService.GetEmailByUserIdAsync(patient.UserId, cancellationToken);
            return ToDto(patient, email);
        }


        public async Task<PatientDto?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Patients.GetByNationalIdAsync(nationalId, cancellationToken);
            if (patient is null) throw new NotFoundException($"Patient with national Id '{nationalId}' not found.");
            var email = await _identityService.GetEmailByUserIdAsync(patient.UserId, cancellationToken);
            return ToDto(patient, email);
        }

        public async Task<PatientDto> CreateAsync(CreatePatientDto patientDto, CancellationToken cancellationToken = default)
        {
            if (await _unitOfWork.Patients.ExistsByNationalIdAsync(patientDto.NationalId, cancellationToken))
                throw new DuplicateEntityException($"A patient with National Id '{patientDto.NationalId}' already exists.");

            var (succeeded, userId, error) = await _identityService.CreateUserAsync(patientDto.Email, patientDto.Password, AppRoles.Patient, cancellationToken);

            var patient = new Patient(
                userId,
                patientDto.NationalId,
                patientDto.FullName,
                patientDto.DateOfBirth,
                patientDto.Gender,
                patientDto.PhoneNumber,
                patientDto.BloodType,
                patientDto.Address
                );

            await _unitOfWork.Patients.AddAsync(patient, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToDto(patient, patientDto.Email);
        }

        public async Task<PatientDto> UpdateAsync(Guid id, UpdatePatientDto patientDto, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Patient), id);
            patient.UpdateContactInfo(patientDto.PhoneNumber, patientDto.Address);
            patient.Gender = patientDto.Gender;
            if (patientDto.BloodType.HasValue)
                patient.SetBloodType(patientDto.BloodType.Value);

            _unitOfWork.Patients.Update(patient);
            await _unitOfWork.SaveChangesAsync();
            var email = await _identityService.GetEmailByUserIdAsync(patient.UserId, cancellationToken);
            return ToDto(patient, email);

        }

        // Soft Delete
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Patient), id);
            _unitOfWork.Patients.Delete(patient); // Soft Delete
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }


        // Hard Delete
        public async Task HardDeleteAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Patients.GetByIdWithDeletedAsync(patientId, cancellationToken);
            if (patient is null)
                throw new NotFoundException("Patient not found.");

            var medicalRecords = await _unitOfWork.MedicalRecords.GetByPatientAsync(patientId, cancellationToken);
            var emergencyContact = await _unitOfWork.EmergencyContact.GetByPatientIdAsync(patientId, cancellationToken);

            using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // Deleting Medical Files will be here after completing it


                foreach (var record in medicalRecords)
                {
                    _unitOfWork.MedicalRecords.HardDelete(record);
                }

                foreach (var contact in emergencyContact)
                {
                    _unitOfWork.EmergencyContact.HardDelete(contact);
                }

                _unitOfWork.Patients.HardDelete(patient);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var isDeleted = await _identityService.DeleteUserAsync(patient.UserId, cancellationToken);
                if (!isDeleted)
                {
                    throw new Exception("Failed to delete user account.");
                }

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }


        public async Task<IReadOnlyList<PatientDto>> GetDeletedPatientsAsync(CancellationToken cancellationToken = default)
        {
            var deletedPatients = await _unitOfWork.Patients.GetDeletedAsync(cancellationToken);
            var patientsDtos = new List<PatientDto>();
            foreach (var patient in deletedPatients)
            {
                var email = await _identityService.GetEmailByUserIdAsync(patient.UserId, cancellationToken);
                patientsDtos.Add(ToDto(patient, email));
            }
            return patientsDtos;
        }

        public async Task RestorePatientAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Patients.GetDeletedByIdAsync(patientId, cancellationToken);
            if (patient is null) throw new NotFoundException("Deleted patient record not found.");
            _unitOfWork.Patients.Update(patient);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }



    }
}
