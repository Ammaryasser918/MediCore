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
    public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            builder.ToTable("Prescriptions");
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.MedicalRecordId);
            builder.Property(p => p.MedicalRecordId).IsRequired();

            builder.Property(p => p.Notes).HasMaxLength(1000);


            builder.HasMany(p => p.PrescriptionItems)
                .WithOne(i => i.Prescription)
                .HasForeignKey(i => i.PrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(p => p.PrescriptionItems)
                .HasField("_prescriptionItems")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

        }
    }
}
