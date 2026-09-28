using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PulseID.Domain.Entities;
using PulseID.Infrastructure.Identity;

public class LabTechnicianConfiguration : IEntityTypeConfiguration<LabTechnician>
{
    public void Configure(EntityTypeBuilder<LabTechnician> builder)
    {
        builder.ToTable("LabTechnicians");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.UserId).IsRequired();
        builder.HasIndex(t => t.UserId).IsUnique();

        builder.Property(t => t.FullName)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(t => t.NationalId)
            .IsRequired();

        builder.HasIndex(t => t.NationalId)
            .IsUnique();

        builder.Property(t => t.LicenseNumber)
            .IsRequired();

        builder.HasIndex(t => t.LicenseNumber)
            .IsUnique();

        builder.Property(t => t.PhoneNumber)
            .IsRequired();

        builder.Property(t => t.PersonalImagePath)
            .HasMaxLength(1000);

        builder.Property(t => t.LicenseDocumentPath)
            .HasMaxLength(1000);

        builder.Property(t => t.Status)
            .IsRequired();

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(t => t.ApprovedByLabAdminId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Lab)
            .WithMany(l => l.LabTechnicians)
            .HasForeignKey(t => t.LabId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}