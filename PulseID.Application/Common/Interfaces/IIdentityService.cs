using PulseID.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, Guid UserId, string? Error)> CreateUserAsync(string emial, string password, string role, CancellationToken cancellationToken = default);
        Task<bool> CheckPasswordAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<Guid> FindUserIdByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<string> GetEmailByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<string> GetUserRoleAsync(Guid userId, CancellationToken cancellationToken = default);

        Task SaveRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiresOn, CancellationToken cancellationToken = default);
        Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken = default);
        Task RevokeRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);

    }
}
