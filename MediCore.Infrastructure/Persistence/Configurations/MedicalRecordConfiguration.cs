using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MediCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Infrastructure.Persistence.Configurations
{
    public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            builder.ToTable("MedicalRecords");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Diagnosis).IsRequired();
            builder.Property(m => m.Symptoms).IsRequired();

            builder.HasIndex(m => m.PatientId);
            builder.HasIndex(m => m.DoctorId);
            builder.HasIndex(m => m.RootRecordId);
            builder.HasIndex(m => m.PreviousVersionId);


            builder.HasOne(m => m.Doctor)
                .WithMany()
                .HasForeignKey(m => m.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Hospital)
                .WithMany()
                .HasForeignKey(m => m.HospitalId)
                .OnDelete(DeleteBehavior.Restrict);


            // Self-Referencing
            builder.HasOne(m => m.PreviousVersion)
                .WithMany()
                .HasForeignKey(m => m.PreviousVersionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);




            builder.HasOne(m => m.Prescription)
                .WithOne(p => p.MedicalRecord)
                .HasForeignKey<Prescription>(p => p.MedicalRecordId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(m => m.MedicalFiles)
                .WithOne(f => f.MedicalRecord)
                .HasForeignKey(f => f.MedicalRecordId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(m => m.MedicalFiles)
                .HasField("_files")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

        }
    }
}
