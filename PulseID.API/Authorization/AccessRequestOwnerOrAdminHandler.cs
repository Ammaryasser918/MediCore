using Microsoft.AspNetCore.Authorization;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.AccessAuthorization;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Common;

namespace PulseID.API.Authorization
{
    public class AccessRequestOwnerOrAdminHandler : AuthorizationHandler<OwnerShipRequirement, AccessRequestDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IPatientService _patientService;

        public AccessRequestOwnerOrAdminHandler(
            ICurrentUserService currentUserService,
            IPatientService patientService)
        {
            _currentUserService = currentUserService;
            _patientService = patientService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OwnerShipRequirement requirement,
            AccessRequestDto resource)
        {
            if (resource is null)
                return;

            if (context.User.IsInRole(AppRoles.SuperAdmin) ||
                context.User.IsInRole(AppRoles.HospitalAdmin) ||
                context.User.IsInRole(AppRoles.LabAdmin) ||
                context.User.IsInRole(AppRoles.PharmacyAdmin))
            {
                context.Succeed(requirement);
                return;
            }

            var currentUserId = _currentUserService.UserId;
            if (currentUserId is null)
                return;

            if (context.User.IsInRole(AppRoles.Patient))
            {
                var currentPatient = await _patientService.GetByUserIdAsync(currentUserId.Value);
                if (currentPatient is not null && resource.PatientId == currentPatient.Id)
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            if (context.User.IsInRole(AppRoles.Doctor) ||
                context.User.IsInRole(AppRoles.LabTechnician) ||
                context.User.IsInRole(AppRoles.Pharmacist))
            {
                if (resource.GranteeUserId == currentUserId)
                {
                    context.Succeed(requirement);
                    return;
                }
            }
        }
    }
}