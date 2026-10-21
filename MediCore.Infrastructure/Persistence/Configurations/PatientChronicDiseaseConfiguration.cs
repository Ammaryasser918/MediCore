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
    public class PatientChronicDiseaseConfiguration : IEntityTypeConfiguration<PatientChronicDisease>
    {
        public void Configure(EntityTypeBuilder<PatientChronicDisease> builder)
        {
            builder.ToTable("PatientChronicDiseases");
            builder.HasKey(pcd => new { pcd.PatientId, pcd.ChronicDiseaseId });

            builder.HasOne(pcd => pcd.ChronicDisease)
                .WithMany()
                .HasForeignKey(pcd => pcd.ChronicDiseaseId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
