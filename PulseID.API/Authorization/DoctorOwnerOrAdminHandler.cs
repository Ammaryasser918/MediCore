using Microsoft.AspNetCore.Authorization;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.Doctor;
using PulseID.Domain.Entities;

namespace PulseID.API.Authorization
{
    public class DoctorOwnerOrAdminHandler : AuthorizationHandler<OwnerShipRequirement, DoctorDto>
    {
        private readonly ICurrentUserService _currentUserService;
        public DoctorOwnerOrAdminHandler(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }


        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerShipRequirement requirement, DoctorDto resource)
        {
            if(context.User.IsInRole("HospitalAdmin") || context.User.IsInRole("SuperAdmin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var currentUserId = _currentUserService.UserId;
            
            if (currentUserId is not null && currentUserId == resource.UserId)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;

        }

    }
}
