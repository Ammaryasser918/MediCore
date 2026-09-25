using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PulseID.Domain.Entities;
using PulseID.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Infrastructure.Persistence.Configurations
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.ToTable("Doctors");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.UserId).IsRequired();
            builder.HasIndex(p => p.UserId).IsUnique();
            builder.Property(p => p.NationalId).IsRequired();
            builder.HasIndex(p => p.NationalId).IsUnique();

            builder.Property(p => p.FullName).IsRequired().HasMaxLength(250);
            builder.Property(p => p.SyndicateNumber).IsRequired();
            builder.Property(p => p.PhoneNumber).IsRequired();
            builder.HasIndex(d => d.SyndicateNumber).IsUnique();


            builder.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(d => d.ApprovedByHospitalAdminId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(d => d.DateOfBirth)
            .IsRequired()
            .HasDefaultValue(new DateTime(1990, 1, 1));

        }
    }
}
