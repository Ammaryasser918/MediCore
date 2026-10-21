using MediCore.Application.Common.Exceptions;
using MediCore.Application.Common.Interfaces;
using MediCore.Application.DTOs.HospitalAdmin;
using MediCore.Application.Interfaces;
using MediCore.Application.Interfaces.IServices;
using MediCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MediCore.Application.Services
{
    public class HospitalAdminService : IHospitalAdminService
    {
        readonly IIdentityService _identityService;
        readonly IUnitOfWork _unitOfWork;
        public HospitalAdminService(IIdentityService identityService, IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        public async Task<HospitalAdminDto> CreateHospitalAdminAsync(CreateHospitalAdminDto dto, CancellationToken cancellationToken = default)
        {
            var userId = await _identityService.CreateHospitalAdminAsync(dto, cancellationToken);
            var hospitalAdmin = new HospitalAdmin(userId, dto.HospitalId);

            await _unitOfWork.HospitalAdmin.AddAsync(hospitalAdmin, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToDto(hospitalAdmin, dto.Email);

        }

        public async Task<HospitalAdminDto> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var admin = await _unitOfWork.HospitalAdmin.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(HospitalAdmin), id);

            var email = await _identityService.GetEmailByUserIdAsync(admin.UserId, cancellationToken);
            return ToDto(admin, email);

        }

        private static HospitalAdminDto ToDto(HospitalAdmin hospitalAdmin, string email)
        {
            return new HospitalAdminDto
            {
                Id = hospitalAdmin.Id,
                Email = email,
                HospitalId = hospitalAdmin.HospitalId
            };
        }
    }
}
