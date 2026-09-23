using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PulseID.Domain.Entities;

namespace PulseID.Infrastructure.Persistence.Configurations;

public class LabTestRequestConfiguration : IEntityTypeConfiguration<LabTestRequest>
{
    public void Configure(EntityTypeBuilder<LabTestRequest> builder)
    {
        builder.ToTable("LabTestRequests");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Notes)
            .HasMaxLength(1000);

        builder.Property(r => r.ResultFileUrl)
            .HasMaxLength(2000);

        builder.Property(r => r.Status)
            .IsRequired();

        builder.HasOne(r => r.Patient)
            .WithMany(p => p.LabTestRequests)
            .HasForeignKey(r => r.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Doctor)
            .WithMany(d => d.LabTestRequests)
            .HasForeignKey(r => r.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.LabTest)
            .WithMany(t => t.TestRequests)
            .HasForeignKey(r => r.LabTestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Lab)
            .WithMany(l => l.TestRequests)
            .HasForeignKey(r => r.LabId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}