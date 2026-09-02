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
        Task<(bool Succeeded, Guid? UserId, string? Error)> CreateUserAsync(string emial, string password, CancellationToken cancellationToken = default);
        Task<bool> CheckPasswordAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<Guid?> FindUserIdByEmailAsync(string email, CancellationToken cancellationToken = default);

    }
}
