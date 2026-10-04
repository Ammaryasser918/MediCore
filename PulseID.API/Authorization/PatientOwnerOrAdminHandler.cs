using Microsoft.AspNetCore.Authorization;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.Patient;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Entities;

namespace PulseID.API.Authorization
{
    public class PatientOwnerOrAdminHandler : AuthorizationHandler<OwnerShipRequirement, PatientDto>
    {
        private readonly ICurrentUserService _currentUserService;
        readonly IPatientService _patientService;

        public PatientOwnerOrAdminHandler(ICurrentUserService currentUserService, IPatientService patientService)
        {
            _currentUserService = currentUserService;
            _patientService = patientService;
        }


        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerShipRequirement requirement, PatientDto resource)
        {
            var currentUserId = _currentUserService.UserId;
            if (currentUserId is null) return;
            if (context.User.IsInRole("SuperAdmin") || context.User.IsInRole("HospitalAdmin") || context.User.IsInRole("Doctor"))
            {
                context.Succeed(requirement);
                return;
            }
            var currentPatient = await _patientService.GetByUserIdAsync(currentUserId.Value);
            if (currentUserId is not null && currentPatient.Id == resource.Id)
                context.Succeed(requirement);
            return;
        }


    }
}
