using Microsoft.AspNetCore.Authorization;
using MediCore.Application.Common.Interfaces;
using MediCore.Application.DTOs.Doctor;
using MediCore.Application.Interfaces.IServices;
using MediCore.Domain.Entities;

namespace MediCore.API.Authorization
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
            if (currentUserId is not null && currentDoctor.Id == resource.Id)
            {
                context.Succeed(requirement);
            }
            return;

        }

    }
}
