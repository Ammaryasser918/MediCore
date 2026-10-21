using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MediCore.Domain.Entities;
using MediCore.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Infrastructure.Persistence.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("Patients");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.NationalId).IsRequired().HasMaxLength(14);
            builder.HasIndex(p => p.UserId).IsUnique();
            builder.HasIndex(p => p.NationalId).IsUnique();

            builder.Property(p => p.FullName).IsRequired().HasMaxLength(250);
            builder.Property(p => p.PhoneNumber).IsRequired();
            builder.Property(p => p.Address).HasMaxLength(500);

            builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);

            // ----- Patient Aggregate ------

            // Emergency Contacts
            builder.HasMany(p => p.EmergencyContacts).WithOne(ec => ec.Patient)
                .HasForeignKey(ec => ec.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(p => p.EmergencyContacts)
                .HasField("_emergencyContacts")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            // Patient Allergies
            builder.HasMany(p => p.PatientAllergies)
                .WithOne(pa => pa.Patient)
                .HasForeignKey(pa => pa.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(p => p.PatientAllergies)
                .HasField("_patientAllergies")
                .UsePropertyAccessMode(PropertyAccessMode.Field);


            // Patient Chronic Diseases
            builder.HasMany(p => p.PatientChronicDiseases)
                .WithOne(pcd => pcd.Patient)
                .HasForeignKey(pcd => pcd.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(p => p.PatientChronicDiseases)
                .HasField("_patientChronicDiseases")
                .UsePropertyAccessMode(PropertyAccessMode.Field);


            // Medical Records
            builder.HasMany(p => p.MedicalRecords)
                .WithOne(m => m.Patient)
                .HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Navigation(p => p.MedicalRecords)
                .HasField("_medicalRecords")
                .UsePropertyAccessMode(PropertyAccessMode.Field);



            // Medical Files
            builder.HasMany(p => p.MedicalFiles)
                .WithOne(f => f.Patient)
                .HasForeignKey(f => f.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Navigation(p => p.MedicalFiles)
                .HasField("_files")
                .UsePropertyAccessMode(PropertyAccessMode.Field);


            // Patient Medications
            builder.HasMany(p => p.PatientMedications)
                .WithOne(pm => pm.Patient)
                .HasForeignKey(pm => pm.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(p => p.PatientMedications)
                .HasField("_patientMedications")
                .UsePropertyAccessMode(PropertyAccessMode.Field);


        }
    }
}
