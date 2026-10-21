using MediCore.Application.Common.Exceptions;
using MediCore.Application.DTOs.Pharmacy;
using MediCore.Application.Interfaces;
using MediCore.Application.Interfaces.IServices;
using MediCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Services
{
    public class PharmacyService : IPharmacyService
    {
        readonly IUnitOfWork _unitOfWork;
        public PharmacyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        static PharmacyDto ToDto(Pharmacy pharmacy) => new()
        {
            Id = pharmacy.Id,
            Name = pharmacy.Name,
            Email = pharmacy.Email,
            Address = pharmacy.Address,
            PhoneNumber = pharmacy.PhoneNumber,
            LicenseNumber = pharmacy.LicenseNumber,
            LicenseDocumentPath = pharmacy.LicenseDocumentPath,
            PharmacyStatus = pharmacy.Status,
            HospitalId = pharmacy.HospitalId,
            HospitalName = pharmacy.Hospital?.Name,
            ApprovedByAdminId = pharmacy.ApprovedByAdminId,
            ApprovedAt = pharmacy.ApprovedAt,
            CreatedAt = pharmacy.CreatedAt,
            UpdatedAt = pharmacy.UpdatedAt,
            RejectedAt = pharmacy.RejectedAt,
            RejectedByAdminId = pharmacy.RejectedByAdminId
        };

        public async Task<IReadOnlyList<PharmacyDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var pharmacies = await _unitOfWork.Pharmacies.GetAllActiveAsync(cancellationToken);
            return pharmacies.Select(ToDto).ToList();
        }

        public async Task<PharmacyDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var pharmacy = await _unitOfWork.Pharmacies.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacy), id);
            return ToDto(pharmacy);
        }

        public async Task<PharmacyDto> CreateAsync(CreatePharmacyDto dto, CancellationToken cancellationToken = default)
        {
            if (dto.HospitalId.HasValue && _unitOfWork.Hospitals.GetByIdAsync(dto.HospitalId.Value, cancellationToken) is null)
                throw new NotFoundException(nameof(Hospital), dto.HospitalId.Value);

            var pharmacy = new Pharmacy(
                dto.Name,
                dto.Email,
                dto.Address,
                dto.PhoneNumber,
                dto.LicenseNumber,
                dto.LicenseDocumentPath,
                dto.HospitalId
                );

            await _unitOfWork.Pharmacies.AddAsync(pharmacy, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToDto(pharmacy);

        }


        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var pharmacy = await _unitOfWork.Pharmacies.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacy), id);
            _unitOfWork.Pharmacies.Delete(pharmacy);
            var pharmacists = await _unitOfWork.Pharmacists.GetByPharmacyIdAsync(id, cancellationToken);
            foreach(var pharmacist in pharmacists)
            {
                pharmacist.SoftDelete();
                _unitOfWork.Pharmacists.Update(pharmacist);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task HardDeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var pharmacy = await _unitOfWork.Pharmacies.GetByIdWithDeletedAsync(id, cancellationToken);
            if (pharmacy is null) throw new NotFoundException("Pharmacy not found.");

            try
            {
                _unitOfWork.Pharmacies.HardDelete(pharmacy);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {
                throw new Exception("Failed to delete pharmacy.");
            }
        }

        public async Task<IReadOnlyList<PharmacyDto>> GetDeletedPharmaciesAsync(CancellationToken cancellationToken = default)
        {
            var deletedPharmacies = await _unitOfWork.Pharmacies.GetDeletedAsync(cancellationToken);
            var pharmacyDtos = new List<PharmacyDto>();
            foreach(var pharmacy in deletedPharmacies)
            {
                pharmacyDtos.Add(ToDto(pharmacy));
            }
            return pharmacyDtos;
        }

        public async Task RestorePharmacyAsync(Guid pharmacyId, CancellationToken cancellationToken = default)
        {
            var pharmacy = await _unitOfWork.Pharmacies.GetDeletedByIdAsync(pharmacyId, cancellationToken);
            if (pharmacy is null) throw new NotFoundException("Deleted pharmacy record not found.");

            _unitOfWork.Pharmacies.Restore(pharmacy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }


        public async Task<PharmacyDto> ApproveAsync(Guid id , Guid adminId, CancellationToken cancellationToken = default)
        {
            var pharmacy = await _unitOfWork.Pharmacies.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof (Pharmacy), id);

            pharmacy.Approve(adminId);
            _unitOfWork.Pharmacies.Update(pharmacy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToDto(pharmacy);

        }


        public async Task<PharmacyDto> RejectAsync(Guid id, Guid adminId, CancellationToken cancellationToken = default)
        {
            var pharmacy = await _unitOfWork.Pharmacies.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacy), id);
            pharmacy.Reject(adminId);
            _unitOfWork.Pharmacies.Update(pharmacy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToDto(pharmacy);
        }

        public async Task<PharmacyDto> DeactivateAsync(Guid id, Guid adminId, CancellationToken cancellationToken = default)
        {
            var pharmacy = await _unitOfWork.Pharmacies.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacy), id);
            pharmacy.Deactivate(adminId);
            var pharmacists = await _unitOfWork.Pharmacists.GetByPharmacyIdAsync(id, cancellationToken);
            foreach (var pharmacist in pharmacists)
            {
                pharmacist.Deactivate(adminId);
                _unitOfWork.Pharmacists.Update(pharmacist);
            }
            _unitOfWork.Pharmacies.Update(pharmacy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToDto(pharmacy);
        }



        public async Task<PharmacyDto> ReactivateAsync(Guid id, Guid adminId, CancellationToken cancellationToken = default)
        {
            var pharmacy = await _unitOfWork.Pharmacies.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacy), id);
            pharmacy.Reactivate(adminId);
            var pharmacists = await _unitOfWork.Pharmacists.GetByPharmacyIdAsync(id, cancellationToken);
            foreach (var pharmacist in pharmacists)
            {
                pharmacist.Reactivate(adminId);
                _unitOfWork.Pharmacists.Update(pharmacist);
            }
            _unitOfWork.Pharmacies.Update(pharmacy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToDto(pharmacy);
        }


    }
}
