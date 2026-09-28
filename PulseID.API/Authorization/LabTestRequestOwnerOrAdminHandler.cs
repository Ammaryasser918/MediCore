using Microsoft.AspNetCore.Authorization;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.Labs;
using PulseID.Application.Interfaces.IServices;

namespace PulseID.API.Authorization
{
    public class LabTestRequestOwnerOrAdminHandler : AuthorizationHandler<OwnerShipRequirement, LabTestRequestDto>
    {

        private readonly ICurrentUserService _currentUserService;

        private readonly IPatientService _patientService;
        public LabTestRequestOwnerOrAdminHandler(ICurrentUserService currentUserService, IPatientService patientService)
        {
            _currentUserService = currentUserService;
            _patientService = patientService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerShipRequirement requirement, LabTestRequestDto resource)
        {
            if (context.User.IsInRole("SuperAdmin")  || context.User.IsInRole("HospitalAdmin") || context.User.IsInRole("LabTechnician") || context.User.IsInRole("LabAdmin"))
            {
                context.Succeed(requirement);
                return;
            }
            var currentUserId = _currentUserService.UserId;
            if (currentUserId is null) return;

            var currentPatient = await _patientService.GetByUserIdAsync(currentUserId.Value);
            if (currentPatient is not null && resource.Id == currentPatient.Id)
                context.Succeed(requirement);
            return;

        }




    }
}
