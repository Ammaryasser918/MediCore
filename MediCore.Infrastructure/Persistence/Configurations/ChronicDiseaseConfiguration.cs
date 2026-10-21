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
    public class ChronicDiseaseConfiguration : IEntityTypeConfiguration<ChronicDisease>
    {
        public void Configure(EntityTypeBuilder<ChronicDisease> builder)
        {
            builder.ToTable("ChronicDiseases");
            builder.HasKey(cd => cd.Id);

            builder.Property(cd => cd.Name).IsRequired().HasMaxLength(250);
            builder.HasIndex(cd => cd.Name).IsUnique();

            builder.Property(cd => cd.Description).HasMaxLength(1000);

        }
    }
}
