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
    public class PharmacyAdminConfiguration : IEntityTypeConfiguration<PharmacyAdmin>
    { 
        public void Configure(EntityTypeBuilder<PharmacyAdmin> builder)
        {
            builder.ToTable("PharmacyAdmins");

            builder.HasKey(pa => pa.Id);

            builder.Property(pa => pa.UserId)
                .IsRequired();

            builder.HasIndex(pa => pa.UserId)
                .IsUnique();

            builder.HasOne(pa => pa.Pharmacy)
                .WithMany()
                .HasForeignKey(pa => pa.PharmacyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
