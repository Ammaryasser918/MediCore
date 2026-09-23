using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PulseID.Domain.Entities;

namespace PulseID.Infrastructure.Persistence.Configurations;

public class LabTestConfiguration : IEntityTypeConfiguration<LabTest>
{
    public void Configure(EntityTypeBuilder<LabTest> builder)
    {
        builder.ToTable("LabTests");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.HasIndex(t => t.Name)
            .IsUnique();

        builder.Property(t => t.Type)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(t => t.TestRequests)
            .WithOne(r => r.LabTest)
            .HasForeignKey(r => r.LabTestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}