using Microsoft.AspNetCore.Authorization;
using MediCore.Application.Common.Interfaces;
using MediCore.Application.DTOs.Patient;
using MediCore.Application.Interfaces.IServices;
using MediCore.Domain.Entities;

namespace MediCore.API.Authorization
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
            if (context.User.IsInRole("SuperAdmin") || context.User.IsInRole("HospitalAdmin") || context.User.IsInRole("Doctor") || context.User.IsInRole("Pharmacist"))
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
