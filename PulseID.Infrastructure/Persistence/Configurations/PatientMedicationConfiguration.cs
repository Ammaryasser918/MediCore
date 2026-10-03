using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PulseID.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Infrastructure.Persistence.Configurations
{
    public class PatientMedicationConfiguration : IEntityTypeConfiguration<PatientMedication>
    {
        public void Configure(EntityTypeBuilder<PatientMedication> builder)
        {
            builder.ToTable("PatientMedications");
            builder.HasKey(pm => pm.Id);

            builder.Property(pm => pm.Dosage).HasMaxLength(100);
            builder.Property(pm => pm.Frequency).HasMaxLength(100);
            builder.Property(pm => pm.IsActive).IsRequired();

            builder.HasOne(pm => pm.Medication)
                .WithMany()
                .HasForeignKey(pm => pm.MedicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(pm => new { pm.PatientId, pm.IsActive });

            builder.HasIndex(pm => new { pm.PatientId, pm.MedicationId })
                .IsUnique()
                .HasFilter("[IsActive] = 1");
        }

    }
}
