using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MediCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Infrastructure.Persistence.Configurations
{
    public class AllergyConfiguration : IEntityTypeConfiguration<Allergy>
    {
        public void Configure(EntityTypeBuilder<Allergy> builder)
        {
            builder.ToTable("Allergies");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name).IsRequired().HasMaxLength(250);
            builder.HasIndex(a => a.Name).IsUnique();

            builder.Property(a => a.Description).HasMaxLength(1500);

        }
    }
}
