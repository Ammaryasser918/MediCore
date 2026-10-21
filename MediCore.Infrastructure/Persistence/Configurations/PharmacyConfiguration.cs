using Microsoft.EntityFrameworkCore;
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
    public class PharmacyConfiguration : IEntityTypeConfiguration<Pharmacy>
    {
        public void Configure(EntityTypeBuilder<Pharmacy> builder)
        {
            builder.ToTable("Pharmacies");
            builder.HasKey(ph => ph.Id);

            builder.Property(ph => ph.Name).IsRequired().HasMaxLength(250);
            builder.Property(ph => ph.Address).HasMaxLength(800);
            builder.Property(ph => ph.PhoneNumber).IsRequired();
            builder.Property(ph => ph.LicenseNumber).IsRequired();

            builder.HasIndex(ph => ph.LicenseNumber).IsUnique();

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(ph => ph.ApprovedByAdminId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ph => ph.Hospital)
                .WithMany()
                .HasForeignKey(ph => ph.HospitalId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(ph => ph.Pharmacists)
                .WithOne(phm => phm.Pharmacy)
                .HasForeignKey(phm => phm.PharmacyId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(ph => ph.Pharmacists)
                .HasField("_pharmacists")
                .UsePropertyAccessMode(PropertyAccessMode.Field);


        }
    }
}
