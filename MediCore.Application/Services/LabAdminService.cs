using MediCore.Application.Common.Exceptions;
using MediCore.Application.Common.Interfaces;
using MediCore.Application.DTOs.LabAdminDto;
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
    public class LabAdminService : ILabAdminService
    {
        readonly IIdentityService _identityService;
        readonly IUnitOfWork _unitOfWork;

        public LabAdminService(IIdentityService identityService, IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        public async Task<LabAdminDto> CreateLabAdminAsync(CreateLabAdminDto dto, CancellationToken cancellationToken = default)
        {
            var userId = await _identityService.CreateLabAdminAsync(dto, cancellationToken);
            var labAdmin = new LabAdmin(userId, dto.LabId);

            await _unitOfWork.LabAdmin.AddAsync(labAdmin, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToDto(labAdmin, dto.Email);
        }

        public async Task<LabAdminDto> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var admin = await _unitOfWork.LabAdmin.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(LabAdmin), id);
            var email = await _identityService.GetEmailByUserIdAsync(admin.UserId);
            return ToDto(admin, email);
        }

        static LabAdminDto ToDto(LabAdmin labAdmin, string email)
        {
            return new LabAdminDto
            {
                Id = labAdmin.Id,
                Email = email,
                LabId = labAdmin.LabId
            };
        }

    }
}
