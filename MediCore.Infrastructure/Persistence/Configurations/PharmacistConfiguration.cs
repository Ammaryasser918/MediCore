using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Identity.Client;
using MediCore.Domain.Entities;
using MediCore.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Infrastructure.Persistence.Configurations
{
    public class PharmacistConfiguration : IEntityTypeConfiguration<Pharmacist>
    {
        public void Configure(EntityTypeBuilder<Pharmacist> builder)
        {
            builder.ToTable("Pharmacists");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.UserId).IsRequired();
            builder.HasIndex(p => p.UserId).IsUnique();
            builder.HasIndex(p => p.NationalId).IsUnique();

            builder.Property(p => p.FullName).IsRequired().HasMaxLength(250);
            builder.Property(p => p.SyndicateNumber).IsRequired();
            builder.Property(p => p.NationalId).IsRequired();
            builder.Property(p => p.PhoneNumber).IsRequired();

            builder.HasIndex(p => p.SyndicateNumber).IsUnique();

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(p => p.ApprovedByAdminId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);



            builder.Navigation(p => p.PrescriptionItems)
                .HasField("_dispensedItems")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Property(d => d.DateOfBirth)
            .IsRequired()
            .HasDefaultValue(new DateTime(1990, 1, 1));


        }
    }
}
