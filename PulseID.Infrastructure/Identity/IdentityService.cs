using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PulseID.Application.Common.Interfaces;

namespace PulseID.Infrastructure.Identity
{
    public partial class IdentityService : IIdentityService
    {
        readonly UserManager<ApplicationUser> _userManager;
        private const string ProviderName = "PulseIDApp";
        private const string RefreshTokenName = "RefreshToken";
        private const string ExpiryTokenName = "RefreshTokenExpiresOn";
        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
            => await _userManager.FindByEmailAsync(email) is not null;

        public async Task<(bool Succeeded, Guid UserId, string? Error)> CreateUserAsync(string email, string password, string role, CancellationToken cancellationToken = default)
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
                return (false, Guid.Empty, error);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                var roleError = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                return (false, Guid.Empty, roleError);
            }

            return (true, user.Id, null);

        }

        public async Task<(bool IsValid, string? Error)> ValidatePasswordsAsync(string password, CancellationToken cancellationToken = default)
        {
            var dummyUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "temp_validation_user",
                Email = "validation@pulseid.com"
            };

            var errors = new List<string>();

            foreach (var validator in _userManager.PasswordValidators)
            {
                var result = await validator.ValidateAsync(_userManager, dummyUser, password);
                if (!result.Succeeded)
                {
                    errors.AddRange(result.Errors.Select(e => e.Description));
                }
            }

            if (errors.Any())
            {
                return (false, string.Join(" | ", errors));
            }

            return (true, null);
        }
        public async Task<bool> CheckPasswordAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null || !user.IsActive)
                return false;

            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<(bool Succeeded, string? Error)> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return (false, "User not found.");
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                var errorMessage = result.Errors.FirstOrDefault()?.Description ?? "Failed to change password.";
                return (false, errorMessage);
            }
            return (true, null);
        }

        public async Task<Guid> FindUserIdByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user.Id;
        }

        public async Task<string> GetEmailByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user.Email;
        }
        public async Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<string> GetUserRoleAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null || !user.IsActive) return null;
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault();
        }


        public async Task SaveRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiresOn, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return;

            await _userManager.SetAuthenticationTokenAsync(user, ProviderName, RefreshTokenName, refreshToken);
            await _userManager.SetAuthenticationTokenAsync(user, ProviderName, ExpiryTokenName, expiresOn.ToString("o"));

        }

        public async Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return false;

            var token = await _userManager.GetAuthenticationTokenAsync(user, ProviderName, RefreshTokenName);
            var expiryStr = await _userManager.GetAuthenticationTokenAsync(user, ProviderName, ExpiryTokenName);

            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(expiryStr))
                return false;

            if (refreshToken != token)
                return false;

            if (DateTime.TryParse(expiryStr, out var expiryDate) && expiryDate <= DateTimeOffset.UtcNow)
            {
                await RevokeRefreshTokenAsync(userId, cancellationToken);
                return false;
            }
            return true;
        }

        public async Task RevokeRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return;
            await _userManager.RemoveAuthenticationTokenAsync(user, ProviderName, RefreshTokenName);
            await _userManager.RemoveAuthenticationTokenAsync(user, ProviderName, ExpiryTokenName);
            
        }

    }
}
