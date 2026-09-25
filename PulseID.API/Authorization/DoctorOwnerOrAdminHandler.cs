using Microsoft.AspNetCore.Authorization;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.Doctor;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Entities;

namespace PulseID.API.Authorization
{
    public class DoctorOwnerOrAdminHandler : AuthorizationHandler<OwnerShipRequirement, DoctorDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDoctorService _doctorService;
        public DoctorOwnerOrAdminHandler(ICurrentUserService currentUserService, IDoctorService doctorService)
        {
            _currentUserService = currentUserService;
            _doctorService = doctorService;
        }


        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerShipRequirement requirement, DoctorDto resource)
        {
            var currentUserId = _currentUserService.UserId;
            if (currentUserId is null) return;
            if(context.User.IsInRole("HospitalAdmin") || context.User.IsInRole("SuperAdmin"))
            {
                context.Succeed(requirement);
                return;
            }

            var currentDoctor = await _doctorService.GetByUserIdAsync(currentUserId.Value);
            if (currentUserId is not null && currentUserId == resource.Id)
            {
                context.Succeed(requirement);
            }
            return;

        }

    }
}
