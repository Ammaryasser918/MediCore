using PulseID.Application.Common.Exceptions;
using PulseID.Application.DTOs.Pharmacist;
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
    public class PharmacistService : IPharmacistService
    {
        readonly IUnitOfWork _unitOfWork;
        public PharmacistService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        static PharmacistDto ToDto(Pharmacist pharmacist) 
            => new()
                    {
                        Id = pharmacist.Id,
                        UserId = pharmacist.UserId,
                        FullName = pharmacist.FullName,
                        SyndicateNumber = pharmacist.SyndicateNumber,
                        SyndicateDocumentPath = pharmacist.SyndicateDocumentPath,
                        NationalId = pharmacist.NationalId,
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
            return pharmacists.Select(ToDto).ToList();
        }

        public async Task<PharmacistDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacist), id);
            return ToDto(pharmacist);
        }

        public async Task<PharmacistDto> CreateAsync(CreatePharmacistDto dto, CancellationToken cancellationToken = default)
        {
            if (await _unitOfWork.Pharmacists.GetByNationalIdAsync(dto.NationalId, cancellationToken) is not null)
                throw new DuplicateEntityException($"A pharmacist with National Id '{dto.NationalId}' already exists.");

            if (await _unitOfWork.Pharmacists.GetBySyndicateNumberAsync(dto.SyndicateNumber) is not null)
                throw new DuplicateEntityException($"A pharmacist with Syndicate Number '{dto.SyndicateNumber}' already exists.");

            var pharmacist = new Pharmacist(
                dto.UserId,
                dto.FullName,
                dto.SyndicateNumber,
                dto.SyndicateDocumentPath,
                dto.NationalId,
                dto.PhoneNumber,
                dto.PharmacyId
                );

            await _unitOfWork.Pharmacists.AddAsync(pharmacist, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToDto(pharmacist);

        }


        public async Task<PharmacistDto> AssignToPharmacyAsync(Guid id, AssignPharmacistToPharmacyDto dto, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(Pharmacist), id);

            pharmacist.AssignToPharmacy(dto.PharmacyId);

            _unitOfWork.Pharmacists.Update(pharmacist);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToDto(pharmacist);
        }


        public async Task<PharmacistDto> UpdatePharmacistDetailsAsync(Guid id, UpdatePharmacistDto dto, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacist), id);
            pharmacist.UpdatePersonalDetails(dto.PhoneNumber);
            _unitOfWork.Pharmacists.Update(pharmacist);
            await _unitOfWork.SaveChangesAsync();
            return ToDto(pharmacist);
        }


        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacist), id);

            _unitOfWork.Pharmacists.Delete(pharmacist); // Soft Delete
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }


        public async Task<PharmacistDto> ApproveAsync(Guid id, Guid pharmacyAdminId, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacist), id);
            pharmacist.Approve(pharmacyAdminId);

            _unitOfWork.Pharmacists.Update(pharmacist);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToDto(pharmacist);

        }

        public async Task<PharmacistDto> RejectAsync(Guid id, Guid pharmacyAdminId, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacist), id);
            pharmacist.Reject(pharmacyAdminId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToDto(pharmacist);
        }

        public async Task<PharmacistDto> DeactivateAsync(Guid id, Guid pharmacyAdminId, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacist), id);
            pharmacist.Deactivate(pharmacyAdminId);
            _unitOfWork.Pharmacists.Update(pharmacist);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToDto(pharmacist);
        }

        public async Task<PharmacistDto> ReactivateAsync(Guid id, Guid pharmacyAdminId, CancellationToken cancellationToken = default)
        {
            var pharmacist = await _unitOfWork.Pharmacists.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacist), id);
            pharmacist.Reactivate(pharmacyAdminId);
            _unitOfWork.Pharmacists.Update(pharmacist);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToDto(pharmacist);
        }


    }
}
