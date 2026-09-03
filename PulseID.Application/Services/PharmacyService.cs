using PulseID.Application.Common.Exceptions;
using PulseID.Application.DTOs.Pharmacy;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Services
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
            UpdatedAt = pharmacy.UpdatedAt
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

        public async Task<PharmacyDto> ApproveAsync(Guid id , Guid adminId, CancellationToken cancellationToken = default)
        {
            var pharmacy = await _unitOfWork.Pharmacies.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof (Pharmacy), id);

            pharmacy.Approve(adminId);
            _unitOfWork.Pharmacies.Update(pharmacy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToDto(pharmacy);

        }


        public async Task<PharmacyDto> RejectAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var pharmacy = await _unitOfWork.Pharmacies.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacy), id);
            pharmacy.Reject();
            _unitOfWork.Pharmacies.Update(pharmacy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToDto(pharmacy);
        }

        public async Task<PharmacyDto> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var pharmacy = await _unitOfWork.Pharmacies.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacy), id);
            pharmacy.Deactivate();
            var pharmacists = await _unitOfWork.Pharmacists.GetByPharmacyIdAsync(id, cancellationToken);
            foreach (var pharmacist in pharmacists)
            {
                pharmacist.Deactivate();
                _unitOfWork.Pharmacists.Update(pharmacist);
            }
            _unitOfWork.Pharmacies.Update(pharmacy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToDto(pharmacy);
        }



        public async Task<PharmacyDto> ReactivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var pharmacy = await _unitOfWork.Pharmacies.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Pharmacy), id);
            pharmacy.Reactivate();
            var pharmacists = await _unitOfWork.Pharmacists.GetByPharmacyIdAsync(id, cancellationToken);
            foreach (var pharmacist in pharmacists)
            {
                pharmacist.Reactivate();
                _unitOfWork.Pharmacists.Update(pharmacist);
            }
            _unitOfWork.Pharmacies.Update(pharmacy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToDto(pharmacy);
        }


    }
}
