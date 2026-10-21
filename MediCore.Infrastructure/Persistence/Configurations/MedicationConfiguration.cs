using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MediCore.Domain.Entities;


namespace MediCore.Infrastructure.Persistence.Configurations;


public class MedicationConfiguration : IEntityTypeConfiguration<Medication>
{
    public void Configure(EntityTypeBuilder<Medication> builder)
    {
        builder.ToTable("Medications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CommercialNameEn)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.CommercialNameAr)
            .HasMaxLength(500);

        builder.Property(x => x.ScientificName)
            .HasMaxLength(1000);

        builder.Property(x => x.Manufacturer)
            .HasMaxLength(500);

        builder.Property(x => x.DrugClass)
            .HasMaxLength(500);

        builder.Property(x => x.Route)
            .HasMaxLength(100);

        builder.Property(x => x.PriceEgp)
            .HasColumnType("decimal(18,2)");

        builder.HasIndex(x => x.CommercialNameEn);
    }
}