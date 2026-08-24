using PulseID.Application.Common.Exceptions;
using PulseID.Application.DTOs.Patient;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Services
{
    public class PatientService : IPatientService
    {
        readonly IUnitOfWork _unitOfWork;
        public PatientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        static PatientDto ToDto(Patient patient)
            => new()
            {
                Id = patient.Id,
                UserId = patient.UserId,
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
            return patients.Select(ToDto).ToList();
        }

        public async Task<PatientDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Patient), id);
            return ToDto(patient);
        }

        public async Task<PatientDto?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Patients.GetByNationalIdAsync(nationalId, cancellationToken);
            return patient is null ? null : ToDto(patient);
        }

        public async Task<PatientDto> CreateAsync(CreatePatientDto patientDto, CancellationToken cancellationToken = default)
        {
            if (await _unitOfWork.Patients.ExistsByNationalIdAsync(patientDto.NationalId, cancellationToken))
                throw new DuplicateEntityException($"A patient with National Id '{patientDto.NationalId}' already exists.");

            var patient = new Patient(
                patientDto.UserId,
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
            return ToDto(patient);
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

            return ToDto(patient);

        }


        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Patient), id);
            _unitOfWork.Patients.Delete(patient); // Soft Delete
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }


    }
}
