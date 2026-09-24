using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PulseID.Domain.Entities;
using PulseID.Infrastructure.Identity;

namespace PulseID.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Pharmacist> Pharmacists => Set<Pharmacist>();
        public DbSet<Hospital> Hospitals => Set<Hospital>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Pharmacy> Pharmacies => Set<Pharmacy>();
        public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
        public DbSet<MedicalFile> MedicalFiles => Set<MedicalFile>();
        public DbSet<Prescription> Prescriptions => Set<Prescription>();
        public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();
        public DbSet<OTP> Otps => Set<OTP>();
        public DbSet<AccessAuthorization> AccessAuthorizations => Set<AccessAuthorization>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Allergy> Allergies => Set<Allergy>();
        public DbSet<ChronicDisease> ChronicDiseases => Set<ChronicDisease>();
        public DbSet<EmergencyContact> EmergencyContacts => Set<EmergencyContact>();
        public DbSet<PatientAllergy> PatientAllergies => Set<PatientAllergy>();
        public DbSet<PatientChronicDisease> PatientChronicDiseases => Set<PatientChronicDisease>();
        public DbSet<HospitalAdmin> HospitalAdmins => Set<HospitalAdmin>();
        public DbSet<PharmacyAdmin> PharmacyAdmin => Set<PharmacyAdmin>();


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }


    }
}
