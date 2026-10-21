using Microsoft.AspNetCore.Http;
using MediCore.Application.Common.Exceptions;
using MediCore.Application.Common.Interfaces;
using MediCore.Application.DTOs.Doctor;
using MediCore.Application.DTOs.Patient;
using MediCore.Application.Interfaces;
using MediCore.Application.Interfaces.IServices;
using MediCore.Domain.Common;
using MediCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Services
{
    public class DoctorService : IDoctorService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IIdentityService _identityService;
        readonly IFileStorageService _fileStorageService;

        public DoctorService(IUnitOfWork unitOfWork, IIdentityService identityService, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _fileStorageService = fileStorageService;
        }

        DoctorDto ToDto(Doctor doctor, string email) => new()
        {
            Id = doctor.Id,
            Email = email,
            FullName = doctor.FullName,
            SyndicateNumber = doctor.SyndicateNumber,
            SyndicateDocumentPath = doctor.SyndicateDocumentPath,
            NationalId = doctor.NationalId,
            Specialization = doctor.Specialization,
            Age = doctor.Age,
            DateOfBirth = doctor.DateOfBirth,
            PhoneNumber = doctor.PhoneNumber,
            PersonalImageUrl = _fileStorageService.GetUrl(doctor.PersonalImagePath),
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


        public async Task<DoctorDto> SetPersonalImageAsync(Guid id, IFormFile file, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Doctor), id);

            var oldPath = doctor.PersonalImagePath;
            var newPath = await _fileStorageService.SaveAsync(file, UploadFolder.PersonalImages, cancellationToken);

            doctor.SetPersonalImage(newPath);
            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _fileStorageService.DeleteAsync(oldPath, cancellationToken);

            var email = await _identityService.GetEmailByUserIdAsync(doctor.UserId, cancellationToken);
            return ToDto(doctor, email);

        }

        public async Task<DoctorDto> RemovePersonalImageAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Doctor), id);

            var oldPath = doctor.PersonalImagePath;

            doctor.RemovePersonalImage();
            await _fileStorageService.DeleteAsync(oldPath, cancellationToken);
            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var email = await _identityService.GetEmailByUserIdAsync(doctor.UserId, cancellationToken);
            return ToDto(doctor, email);
        }

        public async Task<IReadOnlyList<DoctorDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var doctors = await _unitOfWork.Doctors.GetAllAsync(cancellationToken);
            var doctorDtos = new List<DoctorDto>();
            foreach(var doctor in doctors)
            {
                var email = await _identityService.GetEmailByUserIdAsync(doctor.UserId, cancellationToken);
                doctorDtos.Add(ToDto(doctor, email));
            }
            return doctorDtos;
        }

        public async Task<DoctorDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Doctor), id);
            var email = await _identityService.GetEmailByUserIdAsync(doctor.UserId, cancellationToken);
            return ToDto(doctor, email);
        }

        public async Task<DoctorDto> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId, cancellationToken) ?? throw new NotFoundException(nameof(Doctor), userId);
            var email = await _identityService.GetEmailByUserIdAsync(doctor.UserId, cancellationToken);
            return ToDto(doctor, email);
        }


        public async Task<IReadOnlyList<DoctorDto>> GetByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default)
        {
            var doctors = await _unitOfWork.Doctors.GetByDepartmentAsync(departmentId, cancellationToken);
            var doctorDtos = new List<DoctorDto>();
            foreach (var doctor in doctors)
            {
                var email = await _identityService.GetEmailByUserIdAsync(doctor.UserId, cancellationToken);
                doctorDtos.Add(ToDto(doctor, email));
            }
            return doctorDtos;
        }
        
        public async Task<DoctorDto> CreateAsync(CreateDoctorDto doctorDto, CancellationToken cancellationToken = default)
        {
            if (await _unitOfWork.Doctors.GetByNationalIdAsync(doctorDto.NationalId, cancellationToken) is not null)
                throw new DuplicateEntityException($"A doctor with National Id '{doctorDto.NationalId}' already exists.");

            if (await _unitOfWork.Doctors.GetBySyndicateNumberAsync(doctorDto.SyndicateNumber, cancellationToken) is not null)
                throw new DuplicateEntityException($"A doctor with Syndicate Number '{doctorDto.SyndicateNumber}' already exists.");
            var (succeeded, userId, error) = await _identityService.CreateUserAsync(doctorDto.Email, doctorDto.Password, AppRoles.Doctor, cancellationToken);
            var doctor = new Doctor(
                userId,
                doctorDto.FullName,
                doctorDto.SyndicateNumber,
                doctorDto.SyndicateDocumentPath,
                doctorDto.NationalId,
                doctorDto.Specialization,
                doctorDto.PhoneNumber,
                doctorDto.DateOfBirth,
                doctorDto.DepartmentId
                );

            await _unitOfWork.Doctors.AddAsync(doctor, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToDto(doctor, doctorDto.Email);

        }

        public async Task RemoveFromDepartment(Guid doctorId, CancellationToken cancellationToken)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(doctorId, cancellationToken)
                ?? throw new NotFoundException(nameof(Doctor), doctorId);

            doctor.RemoveFromDepartment();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<DoctorDto> UpdateDepartmentAsync(Guid id, UpdateDoctorDepartmentDto dto, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Doctor), id);

            doctor.TransferToDepartment(dto.DepartmentId);

            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var email = await _identityService.GetEmailByUserIdAsync(doctor.UserId, cancellationToken);
            return ToDto(doctor, email);
        }


        public async Task<DoctorDto> UpdateAsync(Guid id, UpdateDoctorDto dto, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Doctor), id);
            doctor.UpdatePersonalDetails(dto.FullName, dto.NationalId, dto.PhoneNumber, dto.Specialization, dto.DateOfBirth);
            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.SaveChangesAsync();
            var email = await _identityService.GetEmailByUserIdAsync(doctor.UserId, cancellationToken);
            return ToDto(doctor, email);
        }

        // Soft Delete
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Doctor), id);
            _unitOfWork.Doctors.Delete(doctor); // Soft Delete
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Hard Delete
        public async Task HardDeleteAsync(Guid doctorId, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdWithDeletedAsync(doctorId, cancellationToken);
            if (doctor is null)
                throw new NotFoundException("Doctor not found.");

            using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                _unitOfWork.Doctors.HardDelete(doctor);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var isDeleted = await _identityService.DeleteUserAsync(doctor.UserId, cancellationToken);
                if (!isDeleted)
                {
                    throw new Exception($"Failed to delete user account.");
                }

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<IReadOnlyList<DoctorDto>> GetDeletedDoctorsAsync(CancellationToken cancellationToken = default)
        {
            var deletedDoctors = await _unitOfWork.Doctors.GetDeletedAsync(cancellationToken);
            var doctorDtos = new List<DoctorDto>();
            foreach (var doctor in deletedDoctors)
            {
                var email = await _identityService.GetEmailByUserIdAsync(doctor.UserId, cancellationToken);
                doctorDtos.Add(ToDto(doctor, email));
            }
            return doctorDtos;
        }

        public async Task RestoreDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Doctors.GetDeletedByIdAsync(doctorId, cancellationToken);
            if (doctor is null) throw new NotFoundException("Deleted doctor record not found.");

            _unitOfWork.Doctors.Restore(doctor);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }


        public async Task<DoctorDto> ApproveAsync(Guid id, Guid hospitalAdminId, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Doctor), id);
            doctor.Approve(hospitalAdminId);
            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var email = await _identityService.GetEmailByUserIdAsync(doctor.UserId, cancellationToken);
            return ToDto(doctor, email);
        }


        public async Task<DoctorDto> RejectAsync(Guid id, Guid hospitalAdminId, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Doctor), id);
            doctor.Reject(hospitalAdminId);
            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var email = await _identityService.GetEmailByUserIdAsync(doctor.UserId, cancellationToken);
            return ToDto(doctor, email);
        }

        public async Task<DoctorDto> DeactivateAsync(Guid id, Guid hospitalAdminId, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Doctor), id);
            doctor.Deactivate(hospitalAdminId);
            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var email = await _identityService.GetEmailByUserIdAsync(doctor.UserId, cancellationToken);
            return ToDto(doctor, email);
        }

        public async Task<DoctorDto> ReactivateAsync(Guid id, Guid hospitalAdminId, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Doctor), id);
            doctor.Reactivate(hospitalAdminId);
            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var email = await _identityService.GetEmailByUserIdAsync(doctor.UserId, cancellationToken);
            return ToDto(doctor, email);
        }


    }
}
