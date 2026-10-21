using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MediCore.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Infrastructure.ExternalServices.JWT
{
    public class JwtTokenService : IJwtTokenService
    {
        readonly JwtOptions _options;
        public JwtTokenService(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public string GenerateToken(Guid userId, string role, string email)
        {
            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, userId.ToString()),
                new (ClaimTypes.Role, role),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new (ClaimTypes.Email, email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);


        }

        public string GenerateRefreshToken()
        {
            var bytes = new byte[64];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);

            return Convert.ToBase64String(bytes);
        }

    }
}
