using Microsoft.AspNetCore.Authorization;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.Patient;
using PulseID.Domain.Entities;

namespace PulseID.API.Authorization
{
    public class PatientOwnerOrAdminHandler : AuthorizationHandler<OwnerShipRequirement, PatientDto>
    {
        private readonly ICurrentUserService _currentUserService;

        public PatientOwnerOrAdminHandler(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }


        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerShipRequirement requirement, PatientDto resource)
        {
            var currentUserId = _currentUserService.UserId;
            if (context.User.IsInRole("SuperAdmin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (currentUserId is not null && resource.UserId == currentUserId)
                context.Succeed(requirement);
            return Task.CompletedTask;
        }


    }
}
