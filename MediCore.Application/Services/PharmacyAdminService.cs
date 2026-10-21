using MediCore.Application.Common.Interfaces;
using MediCore.Application.Common.Exceptions;
using MediCore.Application.DTOs.HospitalAdmin;
using MediCore.Application.DTOs.PharmacyAdmin;
using MediCore.Application.Interfaces;
using MediCore.Application.Interfaces.IServices;
using MediCore.Application.Interfaces.Repositories;
using MediCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Services
{
    public class PharmacyAdminService : IPharmacyAdminService
    {
        readonly IIdentityService _identityService;
        readonly IUnitOfWork _unitOfWork;
        public PharmacyAdminService(IIdentityService identityService, IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        public async Task<PharmacyAdminDto> CreatePharmacyAdminAsync(CreatePharmacyAdminDto dto, CancellationToken cancellationToken = default)
        {
            var userId = await _identityService.CreatePharmacyAdminAsync(dto, cancellationToken);
            var pharmacyAdmin = new PharmacyAdmin(userId, dto.PharmacyId);

            await _unitOfWork.PharmacyAdmin.AddAsync(pharmacyAdmin, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToDto(pharmacyAdmin, dto.Email);

        }

        public async Task<PharmacyAdminDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var admin = await _unitOfWork.PharmacyAdmin.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(PharmacyAdmin), id);

            var email = await _identityService.GetEmailByUserIdAsync(admin.UserId);
            return ToDto(admin, email);
        }

        private static PharmacyAdminDto ToDto(PharmacyAdmin pharmacyAdmin, string email)
        {
            return new PharmacyAdminDto
            {
                Id = pharmacyAdmin.Id,
                Email = email,
                PharmacyId = pharmacyAdmin.PharmacyId
            };
        }
    }
}
