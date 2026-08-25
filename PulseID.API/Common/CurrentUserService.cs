using PulseID.Application.Common.Interfaces;
using System.Security.Claims;

namespace PulseID.API.Common
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {

                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Guid.Parse("12345678-1234-1234-1234-1234567890ab");
                }

                return Guid.Parse(userIdClaim);
            }
        }
        public string? Role
        {
            get
            {
                var roleClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(roleClaim))
                {
                    return "Doctor";
                }

                return roleClaim;
            }
        }
    }
}
