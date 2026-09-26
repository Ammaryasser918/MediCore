using PulseID.Application.Common.Exceptions;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.Pharmacist;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Common;
using PulseID.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Services
{
    public class PharmacistService : IPharmacistService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IIdentityService _identityService;
        public PharmacistService(IUnitOfWork unitOfWork, IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }

        static PharmacistDto ToDto(Pharmacist pharmacist, string email) 
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
                        RejectedAt = pharmacist.RejectedAt
                    };


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
            pharmacist.UpdatePersonalDetails(dto.PhoneNumber, dto.DateOfBirth);
            _unitOfWork.Pharmacists.Update(pharmacist);
            await _unitOfWork.SaveChangesAsync();
            var email = await _identityService.GetEmailByUserIdAsync(pharmacist.UserId, cancellationToken);
            return ToDto(pharmacist, email);
        }

        // Soft Delete
        //public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        //{
        //    var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacist), id);

        //    _unitOfWork.Pharmacists.Delete(pharmacist); // Soft Delete
        //    await _unitOfWork.SaveChangesAsync(cancellationToken);
        //}


        // Hard Delete
        public async Task DeleteAsync(Guid pharmacistId, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(pharmacistId, cancellationToken);
            if (pharmacist is null)
                throw new NotFoundException("Pharmacist not found.");

            using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                _unitOfWork.Pharmacists.Delete(pharmacist);
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


    }
}
