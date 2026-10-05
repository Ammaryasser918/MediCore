using Microsoft.AspNetCore.Authorization;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.Prescriptions;
using PulseID.Application.Interfaces.IServices;

namespace PulseID.API.Authorization
{
    public class PrescriptionOwnerOrAdminHandler : AuthorizationHandler<OwnerShipRequirement, PrescriptionDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;

        public PrescriptionOwnerOrAdminHandler(ICurrentUserService currentUserService, IPatientService patientService, IDoctorService doctorService)
        {
            _currentUserService = currentUserService;
            _patientService = patientService;
            _doctorService = doctorService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerShipRequirement requirement, PrescriptionDto resource)
        {
            if (context.User.IsInRole("SuperAdmin") || context.User.IsInRole("HospitalAdmin") || context.User.IsInRole("PharmacyAdmin") || context.User.IsInRole("Pharmacist"))
            {
                context.Succeed(requirement);
                return;
            }

            var currentUserId = _currentUserService.UserId;
            if (currentUserId is null) return;

            if (context.User.IsInRole("Patient"))
            {
                var currentPatient = await _patientService.GetByUserIdAsync(currentUserId.Value);
                if (currentPatient is not null && resource.PatientId == currentPatient.Id)
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            if (context.User.IsInRole("Doctor"))
            {
                var currentDoctor = await _doctorService.GetByUserIdAsync(currentUserId.Value);
                if (currentDoctor is not null && resource.DoctorId == currentDoctor.Id)
                {
                    context.Succeed(requirement);
                    return;
                }
            }


        }



    }
}
