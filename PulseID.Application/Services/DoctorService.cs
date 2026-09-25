using PulseID.Application.Common.Exceptions;
using PulseID.Application.DTOs.Doctor;
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
    public class DoctorService : IDoctorService
    {
        readonly IUnitOfWork _unitOfWork;
        public DoctorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        static DoctorDto ToDto(Doctor doctor) => new()
        {
            Id = doctor.Id,
            UserId = doctor.UserId,
            FullName = doctor.FullName,
            SyndicateNumber = doctor.SyndicateNumber,
            SyndicateDocumentPath = doctor.SyndicateDocumentPath,
            NationalId = doctor.NationalId,
            Specialization = doctor.Specialization,
            PhoneNumber = doctor.PhoneNumber,
            PersonalImagePath = doctor.PersonalImagePath,
            Status = doctor.Status,
            DepartmentId = doctor.DepartmentId,
            DepartmentName = doctor.Department?.Name,
            ApprovedByHospitalAdminId = doctor.ApprovedByHospitalAdminId,
            ApprovedAt = doctor.ApprovedAt,
            CreatedAt = doctor.CreatedAt,
            UpdatedAt = doctor.UpdatedAt,
            RejectedAt = doctor.RejectedAt,
            RejectedByAdminId = doctor.RejectedByAdminId
        };

        public async Task<IReadOnlyList<DoctorDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var doctors = await _unitOfWork.Doctors.GetAllAsync(cancellationToken);
            return doctors.Select(ToDto).ToList();
        }

        public async Task<DoctorDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Doctor), id);
            return ToDto(doctor);
        }


        public async Task<IReadOnlyList<DoctorDto>> GetByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default)
        {
            var doctors = await _unitOfWork.Doctors.GetByDepartmentAsync(departmentId, cancellationToken);
            return doctors.Select(ToDto).ToList();
        }
        
        public async Task<DoctorDto> CreateAsync(CreateDoctorDto doctorDto, CancellationToken cancellationToken = default)
        {
            if (await _unitOfWork.Doctors.GetByNationalIdAsync(doctorDto.NationalId, cancellationToken) is not null)
                throw new DuplicateEntityException($"A doctor with National Id '{doctorDto.NationalId}' already exists.");

            if (await _unitOfWork.Doctors.GetBySyndicateNumberAsync(doctorDto.SyndicateNumber, cancellationToken) is not null)
                throw new DuplicateEntityException($"A doctor with Syndicate Number '{doctorDto.SyndicateNumber}' already exists.");

            var doctor = new Doctor(
                doctorDto.UserId,
                doctorDto.FullName,
                doctorDto.SyndicateNumber,
                doctorDto.SyndicateDocumentPath,
                doctorDto.NationalId,
                doctorDto.Specialization,
                doctorDto.PhoneNumber,
                doctorDto.DepartmentId,
                doctorDto.PersonalImagePath
                );

            await _unitOfWork.Doctors.AddAsync(doctor, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToDto(doctor);

        }

        public async Task<DoctorDto> UpdateDepartmentAsync(Guid id, UpdateDoctorDepartmentDto dto, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Doctor), id);

            doctor.TransferToDepartment(dto.DepartmentId);

            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToDto(doctor);

        }


        public async Task<DoctorDto> UpdateAsync(Guid id, UpdateDoctorDto dto, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Doctor), id);
            doctor.UpdatePersonalDetails(dto.PhoneNumber, dto.Specialization, dto.PersonalImagePath);
            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.SaveChangesAsync();
            return ToDto(doctor);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Doctor), id);
            _unitOfWork.Doctors.Delete(doctor); // Soft Delete
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }


        public async Task<DoctorDto> ApproveAsync(Guid id, Guid hospitalAdminId, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Doctor), id);
            doctor.Approve(hospitalAdminId);
            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToDto(doctor);
        }


        public async Task<DoctorDto> RejectAsync(Guid id, Guid hospitalAdminId, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Doctor), id);
            doctor.Reject(hospitalAdminId);
            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToDto(doctor);
        }

        public async Task<DoctorDto> DeactivateAsync(Guid id, Guid hospitalAdminId, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Doctor), id);
            doctor.Deactivate(hospitalAdminId);
            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToDto(doctor);
        }

        public async Task<DoctorDto> ReactivateAsync(Guid id, Guid hospitalAdminId, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Doctor), id);
            doctor.Reactivate(hospitalAdminId);
            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToDto(doctor);
        }


    }
}
