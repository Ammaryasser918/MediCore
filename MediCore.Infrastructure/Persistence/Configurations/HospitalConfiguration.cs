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
    public class HospitalConfiguration : IEntityTypeConfiguration<Hospital>
    {
        public void Configure(EntityTypeBuilder<Hospital> builder)
        {
            builder.ToTable("Hospitals");
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Name).IsRequired().HasMaxLength(200);
            builder.Property(h => h.Address).HasMaxLength(500);
            builder.Property(h => h.PhoneNumber).IsRequired();
            builder.Property(h => h.LicenseNumber).IsRequired();

            builder.HasIndex(h => h.LicenseNumber).IsUnique();


            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(h => h.ApprovedByAdminId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(h => h.Departments)
                .WithOne(d => d.Hospital)
                .HasForeignKey(d => d.HospitalId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(h => h.Departments)
                .HasField("_departments")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

        }
    }
}
