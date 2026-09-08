using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PulseID.Application.Common.Interfaces;

namespace PulseID.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        readonly UserManager<ApplicationUser> _userManager;

        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
            => await _userManager.FindByEmailAsync(email) is not null;

        public async Task<(bool Succeeded, Guid? UserId, string? Error)> CreateUserAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var error = string.Join("; ", result.Errors.Select(e => e.Description));
                return (false, null, error);
            }

            return (true, user.Id, null);

        }


        public async Task<bool> CheckPasswordAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null || !user.IsActive)
                return false;

            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<Guid?> FindUserIdByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user?.Id;
        }

        public async Task<string?> GetEmailByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user?.Email;
        }

    }
}
