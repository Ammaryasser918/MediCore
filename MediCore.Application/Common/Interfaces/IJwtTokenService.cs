using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Common.Interfaces
{
    public interface IJwtTokenService
    {
        // Signed JWT carrying the given UserId and role as claims
        string GenerateToken(Guid userId, string role, string email);
        string GenerateRefreshToken();
    }
}
