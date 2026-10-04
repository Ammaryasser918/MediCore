using Microsoft.AspNetCore.Authorization;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.MedicalRecord;
using PulseID.Application.Interfaces.IServices;
using PulseID.Application.Services;

namespace PulseID.API.Authorization
{
    public class MedicalRecordOwnerOrAdminHandler : AuthorizationHandler<OwnerShipRequirement, MedicalRecordDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IPatientService _patientService;

        public MedicalRecordOwnerOrAdminHandler(ICurrentUserService currentUserService, IPatientService patientService)
        {
            _currentUserService = currentUserService;
            _patientService = patientService;
        }


        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerShipRequirement requirement, MedicalRecordDto resource)
        {
            if (context.User.IsInRole("SuperAdmin") || 
                context.User.IsInRole("HospitalAdmin") || 
                context.User.IsInRole("Doctor") || 
                context.User.IsInRole("Pharmacist") || 
                context.User.IsInRole("PharmacyAdmin") || 
                context.User.IsInRole("LabTechnician") || 
                context.User.IsInRole("LabAdmin"))
            {
                context.Succeed(requirement);
                return;
            }

            var currentUserId = _currentUserService.UserId;
            if (currentUserId is null) return;
            var currentPatient = await _patientService.GetByUserIdAsync(currentUserId.Value);
            if (currentPatient is not null && (resource.PatientId == currentPatient.Id))
            {
                context.Succeed(requirement);
            }
            return;

        }


    }
}
