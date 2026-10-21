using Microsoft.Extensions.DependencyInjection;
using MediCore.Application.Common.Exceptions;
using MediCore.Application.Common.Interfaces;
using MediCore.Application.Common.Extentions;
using MediCore.Application.DTOs.Doctor;
using MediCore.Application.DTOs.Pharmacist;
using MediCore.Application.Interfaces;
using MediCore.Application.Interfaces.IServices;
using MediCore.Domain.Common;
using MediCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;

namespace MediCore.Application.Services
{
    public class PharmacistService : IPharmacistService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IIdentityService _identityService;
        readonly ICurrentUserService _currentUserService;
        readonly IFileStorageService _fileStorageService;

        public PharmacistService(IUnitOfWork unitOfWork, IIdentityService identityService, ICurrentUserService currentUserService, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _currentUserService = currentUserService;
            _fileStorageService = fileStorageService;
        }

        PharmacistDto ToDto(Pharmacist pharmacist, string email) 
            => new()
                    {
                        Id = pharmacist.Id,
                        Email = email,
                        FullName = pharmacist.FullName,
                        SyndicateNumber = pharmacist.SyndicateNumber,
                        SyndicateDocumentPath = pharmacist.SyndicateDocumentPath,
                        NationalId = pharmacist.NationalId,
                        Age = pharmacist.Age,
                        DateOfBirth = pharmacist.DateOfBirth,
                        Status = pharmacist.Status,
                        PhoneNumber = pharmacist.PhoneNumber,
                        PharmacyId = pharmacist.PharmacyId,
                        PharmacyName = pharmacist.Pharmacy?.Name,
                        ApprovedByAdminId = pharmacist.ApprovedByAdminId,
                        ApprovedAt = pharmacist.ApprovedAt,
                        CreatedAt = pharmacist.CreatedAt,
                        UpdatedAt = pharmacist.UpdatedAt,
                        RejectedByAdminId = pharmacist.RejectedByAdminId,
                        RejectedAt = pharmacist.RejectedAt,
                        PersonalImageUrl = _fileStorageService.GetUrl(pharmacist.PersonalImagePath)
            };

        public async Task<PharmacistDto> SetPersonalImageAsync(Guid id, IFormFile file, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacist), id);

            var oldPath = pharmacist.PersonalImagePath;
            var newPath = await _fileStorageService.SaveAsync(file, UploadFolder.PersonalImages, cancellationToken);

            pharmacist.SetPersonalImage(newPath);
            _unitOfWork.Pharmacists.Update(pharmacist);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _fileStorageService.DeleteAsync(oldPath, cancellationToken);

            var email = await _identityService.GetEmailByUserIdAsync(pharmacist.UserId, cancellationToken);
            return ToDto(pharmacist, email);
        }

        public async Task<PharmacistDto> RemovePersonalImageAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacist), id);
            var oldPath = pharmacist.PersonalImagePath;
            pharmacist.RemovePersonalImage();
            _unitOfWork.Pharmacists.Update(pharmacist);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _fileStorageService.DeleteAsync(oldPath, cancellationToken);
            var email = await _identityService.GetEmailByUserIdAsync(pharmacist.UserId, cancellationToken);
            return ToDto(pharmacist, email);
        }


        public async Task<IReadOnlyList<PharmacistDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var pharmacists = await _unitOfWork.Pharmacists.GetAllActiveAsync(cancellationToken);
            var pharmacistDtos = new List<PharmacistDto>();
            foreach(var pharmacist in pharmacists)
            {
                var email = await _identityService.GetEmailByUserIdAsync(pharmacist.UserId, cancellationToken);
                pharmacistDtos.Add(ToDto(pharmacist, email));
            }
            return pharmacistDtos;
        }

        public async Task<PharmacistDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacist), id);
            var email = await _identityService.GetEmailByUserIdAsync(pharmacist.UserId, cancellationToken);
            return ToDto(pharmacist, email);
        }

        public async Task<PharmacistDto> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetByUserIdAsync(userId, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacist), userId);
            var email = await _identityService.GetEmailByUserIdAsync(pharmacist.UserId, cancellationToken);
            return ToDto(pharmacist, email);
        }

        public async Task<PharmacistDto> CreateAsync(CreatePharmacistDto dto, CancellationToken cancellationToken = default)
        {
            if (await _unitOfWork.Pharmacists.GetByNationalIdAsync(dto.NationalId, cancellationToken) is not null)
                throw new DuplicateEntityException($"A pharmacist with National Id '{dto.NationalId}' already exists.");

            if (await _unitOfWork.Pharmacists.GetBySyndicateNumberAsync(dto.SyndicateNumber) is not null)
                throw new DuplicateEntityException($"A pharmacist with Syndicate Number '{dto.SyndicateNumber}' already exists.");
            var (succeeded, userId, error) = await _identityService.CreateUserAsync(dto.Email, dto.Password, AppRoles.Pharmacist, cancellationToken);
            var pharmacist = new Pharmacist(
                userId,
                dto.FullName,
                dto.SyndicateNumber,
                dto.SyndicateDocumentPath,
                dto.NationalId,
                dto.DateOfBirth,
                dto.PhoneNumber,
                dto.PharmacyId
                );

            await _unitOfWork.Pharmacists.AddAsync(pharmacist, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToDto(pharmacist, dto.Email);

        }


        public async Task<PharmacistDto> AssignToPharmacyAsync(Guid id, AssignPharmacistToPharmacyDto dto, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(Pharmacist), id);

            pharmacist.AssignToPharmacy(dto.PharmacyId);

            _unitOfWork.Pharmacists.Update(pharmacist);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var email = await _identityService.GetEmailByUserIdAsync(pharmacist.UserId, cancellationToken);
            return ToDto(pharmacist, email);
        }


        public async Task<PharmacistDto> UpdatePharmacistDetailsAsync(Guid id, UpdatePharmacistDto dto, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacist), id);
            pharmacist.UpdatePersonalDetails(dto.FullName, dto.NationalId, dto.PhoneNumber, dto.DateOfBirth);
            _unitOfWork.Pharmacists.Update(pharmacist);
            await _unitOfWork.SaveChangesAsync();
            var email = await _identityService.GetEmailByUserIdAsync(pharmacist.UserId, cancellationToken);
            return ToDto(pharmacist, email);
        }

        // Soft Delete
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacist), id);

            _unitOfWork.Pharmacists.Delete(pharmacist); // Soft Delete
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }


        // Hard Delete
        public async Task HardDeleteAsync(Guid pharmacistId, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdWithDeletedAsync(pharmacistId, cancellationToken);
            if (pharmacist is null)
                throw new NotFoundException("Pharmacist not found.");

            using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                _unitOfWork.Pharmacists.HardDelete(pharmacist);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var isDeleted = await _identityService.DeleteUserAsync(pharmacist.UserId, cancellationToken);
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

        public async Task<PharmacistDto> ApproveAsync(Guid id, Guid pharmacyAdminId, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacist), id);
            pharmacist.Approve(pharmacyAdminId);

            _unitOfWork.Pharmacists.Update(pharmacist);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var email = await _identityService.GetEmailByUserIdAsync(pharmacist.UserId, cancellationToken);
            return ToDto(pharmacist, email);

        }

        public async Task<PharmacistDto> RejectAsync(Guid id, Guid pharmacyAdminId, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacist), id);
            pharmacist.Reject(pharmacyAdminId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var email = await _identityService.GetEmailByUserIdAsync(pharmacist.UserId, cancellationToken);
            return ToDto(pharmacist, email);
        }

        public async Task<PharmacistDto> DeactivateAsync(Guid id, Guid pharmacyAdminId, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacist), id);
            pharmacist.Deactivate(pharmacyAdminId);
            _unitOfWork.Pharmacists.Update(pharmacist);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var email = await _identityService.GetEmailByUserIdAsync(pharmacist.UserId, cancellationToken);
            return ToDto(pharmacist, email);
        }

        public async Task<PharmacistDto> ReactivateAsync(Guid id, Guid pharmacyAdminId, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacist), id);
            pharmacist.Reactivate(pharmacyAdminId);
            _unitOfWork.Pharmacists.Update(pharmacist);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var email = await _identityService.GetEmailByUserIdAsync(pharmacist.UserId, cancellationToken);
            return ToDto(pharmacist, email);
        }


        public async Task StartShiftAsync(Guid? pharmacistId, CancellationToken cancellationToken = default)
        {
            var resolvedPharmacistId = await ResolvePharmacistIdAsync(pharmacistId, cancellationToken);
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(resolvedPharmacistId, cancellationToken)
                ?? throw new NotFoundException("Pharmacist not found.");
            pharmacist.StartShift();
            _unitOfWork.Pharmacists.Update(pharmacist);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task EndShiftAsync(Guid? pharmacistId, CancellationToken cancellationToken = default)
        {
            var resolvedPharmacistId = await ResolvePharmacistIdAsync(pharmacistId, cancellationToken);
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(resolvedPharmacistId, cancellationToken)
                ?? throw new NotFoundException("Pharmacist not found.");
            pharmacist.EndShift();
            _unitOfWork.Pharmacists.Update(pharmacist);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }


        public async Task<PharmacistShiftInfoDto> GetShiftInfoAsync(Guid? pharmacistId, CancellationToken cancellationToken = default)
        {
            var resolvedPharmacistId = await ResolvePharmacistIdAsync(pharmacistId, cancellationToken);

            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(resolvedPharmacistId, cancellationToken)
                ?? throw new NotFoundException("Pharmacist not found.");

            var isActive = pharmacist.IsShiftActive;

            TimeSpan? duration = null;
            if (pharmacist.ShiftStartedAt.HasValue)
            {
                var endTime = isActive ? DateTime.UtcNow : pharmacist.ShiftEndedAt;
                if (endTime.HasValue)
                    duration = endTime.Value - pharmacist.ShiftStartedAt.Value;
            }

            string status = !pharmacist.ShiftStartedAt.HasValue
                ? "Not Started"
                : (isActive ? "Active" : "Ended");

            return new PharmacistShiftInfoDto
            {
                PharmacistId = pharmacist.Id,
                ShiftStartedAt = pharmacist.ShiftStartedAt.ToEgyptTime(),
                ShiftEndedAt = pharmacist.ShiftEndedAt.ToEgyptTime(),
                IsActive = isActive,
                Status = status,
                WorkingDuration = duration.HasValue
                    ? $"{duration.Value.Hours:D2}h {duration.Value.Minutes:D2}m"
                    : null
            };
        }

        public async Task<IReadOnlyList<PharmacistDto>> GetDeletedPharmacistsAsync(CancellationToken cancellationToken = default)
        {
            var deletedPharmacists = await _unitOfWork.Pharmacists.GetDeletedAsync(cancellationToken);
            var pharmacistDtos = new List<PharmacistDto>();
            foreach (var pharmacist in deletedPharmacists)
            {
                var email = await _identityService.GetEmailByUserIdAsync(pharmacist.UserId, cancellationToken);
                pharmacistDtos.Add(ToDto(pharmacist, email));
            }
            return pharmacistDtos;
        }

        public async Task RestorePharmacistAsync(Guid pharmacistId, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetDeletedByIdAsync(pharmacistId, cancellationToken);
            if (pharmacist is null) throw new NotFoundException("Deleted pharmacist record not found.");

            _unitOfWork.Pharmacists.Restore(pharmacist);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }




        // helpers
        private Guid RequireCurrentUserId()
    => _currentUserService.UserId ?? throw new AuthenticationRequiredException();

        private async Task<Guid> ResolvePharmacistIdAsync(Guid? pharmacistId, CancellationToken cancellationToken)
        {
            if (pharmacistId.HasValue && pharmacistId.Value != Guid.Empty)
                return pharmacistId.Value;

            var currentUserId = RequireCurrentUserId();
            var role = _currentUserService.Role;

            if (string.Equals(role, AppRoles.Pharmacist, StringComparison.OrdinalIgnoreCase))
            {
                var pharmacist = await _unitOfWork.Pharmacists.GetByUserIdAsync(currentUserId, cancellationToken)
                    ?? throw new NotFoundException($"Pharmacist profile not found for user ID: '{currentUserId}'.");
                return pharmacist.Id;
            }

            throw new ArgumentException("PharmacistId query parameter is required for non-pharmacist callers.");
        }

    }
}
