using Microsoft.AspNetCore.Authorization;
using MediCore.Application.Common.Interfaces;
using MediCore.Application.DTOs.Labs;
using MediCore.Application.Interfaces.Services;

namespace MediCore.API.Authorization
{
    public class LabTechnicianOwnerOrAdminHandler : AuthorizationHandler<OwnerShipRequirement, LabTechnicianDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ILabTechnicianService _labTechnicianService;

        public LabTechnicianOwnerOrAdminHandler(ICurrentUserService currentUserService, ILabTechnicianService labTechnicianService)
        {
            _currentUserService = currentUserService;
            _labTechnicianService = labTechnicianService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerShipRequirement requirement, LabTechnicianDto resource)
        {
            if (context.User.IsInRole("SuperAdmin") || context.User.IsInRole("HospitalAdmin") || context.User.IsInRole("LabAdmin"))
            {
                context.Succeed(requirement);
                return;
            }
            var currentUserId = _currentUserService.UserId;
            if (currentUserId is null) return;
            var currentTechnician = await _labTechnicianService.GetByUserIdAsync(currentUserId.Value);
            if (currentTechnician is not null && currentTechnician.Id == resource.Id)
                context.Succeed(requirement);
            return;
        }



    }
}
