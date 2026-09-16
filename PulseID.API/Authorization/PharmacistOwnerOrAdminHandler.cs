using Microsoft.AspNetCore.Authorization;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.Pharmacist;
using PulseID.Domain.Entities;

namespace PulseID.API.Authorization
{
    public class PharmacistOwnerOrAdminHandler : AuthorizationHandler<OwnerShipRequirement, PharmacistDto>
    {
        private readonly ICurrentUserService _currentUserService;
        public PharmacistOwnerOrAdminHandler(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }


        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerShipRequirement requirement, PharmacistDto resource)
        {
            if (context.User.IsInRole("SuperAdmin") || context.User.IsInRole("PharmacyAdmin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var currentUserId = _currentUserService.UserId;
            if (currentUserId is not null && currentUserId == resource.UserId)
                context.Succeed(requirement);
            return Task.CompletedTask;

        }



    }
}
