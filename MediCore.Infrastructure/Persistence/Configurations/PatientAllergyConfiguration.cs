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
    public class PatientAllergyConfiguration : IEntityTypeConfiguration<PatientAllergy>
    {
        public void Configure(EntityTypeBuilder<PatientAllergy> builder)
        {
            builder.ToTable("PatientAllergies");
            builder.HasKey(pa => new { pa.PatientId, pa.AllergyId });

            builder.HasOne(pa => pa.Allergy)
                .WithMany()
                .HasForeignKey(pa => pa.AllergyId)
                .OnDelete(DeleteBehavior.Restrict);
            
        }
    }
}
