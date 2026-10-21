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
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name).IsRequired().HasMaxLength(200);
            builder.Property(d => d.Description).HasMaxLength(1200);

            builder.HasIndex(d => new { d.HospitalId, d.Name }).IsUnique();

            builder.HasMany(d => d.Doctors)
                .WithOne(doc => doc.Department)
                .HasForeignKey(doc => doc.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Navigation(d => d.Doctors)
                .HasField("_doctors")
                .UsePropertyAccessMode(PropertyAccessMode.Field);


        }
    }
}
