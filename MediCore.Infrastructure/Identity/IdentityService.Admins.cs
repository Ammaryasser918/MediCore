using MediCore.Application.DTOs.HospitalAdmin;
using MediCore.Application.DTOs.LabAdminDto;
using MediCore.Application.DTOs.Pharmacist;
using MediCore.Application.DTOs.PharmacyAdmin;
using MediCore.Application.Interfaces;
using MediCore.Domain.Common;
using MediCore.Domain.Entities;
using MediCore.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Infrastructure.Identity
{
    public partial class IdentityService 
    {
        
        public async Task<Guid> CreateHospitalAdminAsync(CreateHospitalAdminDto dto, CancellationToken cancellationToken = default)
        {
            var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                throw new Exception("Failed to create user : " + string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, AppRoles.HospitalAdmin);

            return user.Id;

        }

        public async Task<Guid> CreatePharmacyAdminAsync(CreatePharmacyAdminDto dto, CancellationToken cancellationToken = default)
        {
            var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                throw new Exception("Failed to create user : " + string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, AppRoles.HospitalAdmin);

            return user.Id;
        }

        public async Task<Guid> CreateLabAdminAsync(CreateLabAdminDto dto, CancellationToken cancellationToken = default)
        {
            var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                throw new Exception("Failed to create user : " + string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, AppRoles.LabAdmin);

            return user.Id;
        }
    }
}
