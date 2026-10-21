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
    public class EmergencyContactConfiguration : IEntityTypeConfiguration<EmergencyContact>
    {
        public void Configure(EntityTypeBuilder<EmergencyContact> builder)
        {
            builder.ToTable("EmergencyContacts");
            builder.HasKey(ec => ec.Id);

            builder.Property(ec => ec.FullName).IsRequired().HasMaxLength(250);
            builder.Property(ec => ec.Relationship).HasMaxLength(200);
            builder.Property(ec => ec.PhoneNumber).IsRequired();
            builder.Property(ec => ec.Address).HasMaxLength(800);
        }
    }
}
