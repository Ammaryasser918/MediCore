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
    public class PrescriptionItemConfiguration : IEntityTypeConfiguration<PrescriptionItem>
    {
        public void Configure(EntityTypeBuilder<PrescriptionItem> builder)
        {
            builder.ToTable("PrescriptionItems");
            builder.HasKey(i => i.Id);

            builder.HasIndex(i => i.PrescriptionId);
            builder.HasIndex(i => i.DispensedByPharmacistId);

            builder.Property(i => i.MedicationCode).IsRequired();
            builder.Property(i => i.MedicationName).IsRequired().HasMaxLength(250);
            builder.Property(i => i.Dosage).IsRequired();
            builder.Property(i => i.Instructions).HasMaxLength(1000);

            builder.HasOne(i => i.DispensedbyPharmacist)
                .WithMany(ph => ph.PrescriptionItems)
                .HasForeignKey(i => i.DispensedByPharmacistId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
