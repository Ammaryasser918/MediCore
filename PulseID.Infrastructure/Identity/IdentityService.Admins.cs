using PulseID.Application.DTOs.HospitalAdmin;
using PulseID.Application.DTOs.Pharmacist;
using PulseID.Application.DTOs.PharmacyAdmin;
using PulseID.Application.Interfaces;
using PulseID.Domain.Common;
using PulseID.Domain.Entities;
using PulseID.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Infrastructure.Identity
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
    }
}
