using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MediCore.Domain.Entities;
using MediCore.Infrastructure.Identity;

namespace MediCore.Infrastructure.Persistence.Configurations;

public class LabConfiguration : IEntityTypeConfiguration<Lab>
{
    public void Configure(EntityTypeBuilder<Lab> builder)
    {
        builder.ToTable("Labs");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(l => l.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(l => l.PhoneNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(l => l.LicenseNumber)
            .HasMaxLength(100);

        builder.Property(l => l.LicenseDocumentPath)
            .HasMaxLength(1000);

        builder.Property(l => l.Status)
            .IsRequired();

        builder.HasMany(l => l.TestRequests)
            .WithOne(r => r.Lab)
            .HasForeignKey(r => r.LabId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(l => l.ApprovedByLabAdminId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}