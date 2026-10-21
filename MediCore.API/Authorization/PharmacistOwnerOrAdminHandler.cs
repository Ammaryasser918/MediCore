using Microsoft.AspNetCore.Authorization;
using MediCore.Application.Common.Interfaces;
using MediCore.Application.DTOs.Pharmacist;
using MediCore.Application.Interfaces.IServices;
using MediCore.Domain.Entities;

namespace MediCore.API.Authorization
{
    public class PharmacistOwnerOrAdminHandler : AuthorizationHandler<OwnerShipRequirement, PharmacistDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IPharmacistService _pharmacistService;
        public PharmacistOwnerOrAdminHandler(ICurrentUserService currentUserService, IPharmacistService pharmacistService)
        {
            _currentUserService = currentUserService;
            _pharmacistService = pharmacistService;
        }


        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerShipRequirement requirement, PharmacistDto resource)
        {
            var currentUserId = _currentUserService.UserId;
            if (currentUserId is null) return;
            if (context.User.IsInRole("SuperAdmin") || context.User.IsInRole("PharmacyAdmin"))
            {
                context.Succeed(requirement);
                return;
            }
            var currentPharmacist = await _pharmacistService.GetByUserIdAsync(currentUserId.Value);
            if (currentUserId is not null && currentPharmacist.Id == resource.Id)
                context.Succeed(requirement);
            return;

        }



    }
}
