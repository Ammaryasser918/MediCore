using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MediCore.Application.Common.Interfaces;
using MediCore.Application.Interfaces.IServices;
using MediCore.Application.Interfaces.Repositories;
using MediCore.Application.Interfaces.Services;
using MediCore.Application.Services;

namespace MediCore.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IPharmacistService, PharmacistService>();
            services.AddScoped<IMedicalRecordService, MedicalRecordService>();
            services.AddScoped<IAccessValidationService, AccessValidationService>();
            services.AddScoped<IMedicalFileService, MedicalFileService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAccessRequestService, AccessRequestService>();
            services.AddScoped<IPharmacyService, PharmacyService>();
            services.AddScoped<IAllergyService, AllergyService>();
            services.AddScoped<IChronicDiseaseService, ChronicDiseaseService>();
            services.AddScoped<IPatientMedicalConditionService, PatientMedicalConditionService>();
            services.AddScoped<IOTPService, OTPService>();
            services.AddScoped<IPharmacyAdminService, PharmacyAdminService>();
            services.AddScoped<IHospitalAdminService, HospitalAdminService>();
            services.AddScoped<IEmergencyContactService, EmergencyContactService>();
            services.AddScoped<ILabTechnicianService, LabTechnicianService>();
            services.AddScoped<IMedicationService, MedicationService>();
            services.AddScoped<ILabAdminService, LabAdminService>();
            services.AddScoped<ILabService, LabService>();
            services.AddScoped<ILabTestService, LabTestService>();
            services.AddScoped<ILabTestRequestService, LabTestRequestService>();
            services.AddScoped<IPatientMedicationService, PatientMedicationService>();
            services.AddScoped<IPrescriptionService, PrescriptionService>();


            services.AddScoped<IHospitalService, HospitalService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            return services;
        }
    }
}
