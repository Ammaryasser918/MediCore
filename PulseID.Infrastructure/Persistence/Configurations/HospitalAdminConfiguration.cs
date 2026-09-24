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
    public class HospitalAdminConfiguration : IEntityTypeConfiguration<HospitalAdmin>
    {
        public void Configure(EntityTypeBuilder<HospitalAdmin> builder)
        {
            builder.ToTable("HospitalAdmins");

            builder.HasKey(ha => ha.Id);

            builder.Property(ha => ha.UserId)
                .IsRequired();

            builder.HasIndex(ha => ha.UserId)
                .IsUnique();

            builder.HasOne(ha => ha.Hospital)
                .WithMany()
                .HasForeignKey(ha => ha.HospitalId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
