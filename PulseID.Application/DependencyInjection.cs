using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.Interfaces.IServices;
using PulseID.Application.Services;

namespace PulseID.Application
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


            return services;
        }
    }
}
