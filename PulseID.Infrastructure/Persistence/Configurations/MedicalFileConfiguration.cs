using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PulseID.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Infrastructure.Persistence.Configurations
{
    public class MedicalFileConfiguration : IEntityTypeConfiguration<MedicalFile>
    {
        public void Configure(EntityTypeBuilder<MedicalFile> builder)
        {
            builder.ToTable("MedicalFiles");
            builder.HasKey(f => f.Id);

            builder.Property(f => f.FileName).IsRequired().HasMaxLength(250);
            builder.Property(f => f.FilePath).IsRequired();
            builder.Property(f => f.ContentType).IsRequired();
            builder.Property(f => f.Description).HasMaxLength(1000);
        }
    }
}
