using MediCore.Application.Common.Interfaces;
using System.Security.Claims;
namespace MediCore.API.Common
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
                var httpContext = _httpContextAccessor.HttpContext;

                // Prefer a real JWT claim once authentication is wired up.
                var userIdValue = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? httpContext?.User?.FindFirst("sub")?.Value;

                return Guid.TryParse(userIdValue, out var userId) ? userId : null;
            }
        }
        public string? Role
        {
            get
            {
                var httpContext = _httpContextAccessor.HttpContext;

                return httpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            }
        }
    }
}