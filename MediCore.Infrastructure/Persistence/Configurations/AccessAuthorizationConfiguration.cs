using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MediCore.Domain.Entities;
using MediCore.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Infrastructure.Persistence.Configurations
{
    public class AccessAuthorizationConfiguration : IEntityTypeConfiguration<AccessAuthorization>
    {
        public void Configure(EntityTypeBuilder<AccessAuthorization> builder)
        {
            builder.ToTable("AccessAuthorizations");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.GranteeRole).IsRequired();
            builder.Property(a => a.RejectionReason).HasMaxLength(1000);

            builder.HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(a => a.GranteeUserId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(a => new { a.PatientId, a.GranteeUserId, a.Status });

        }
    }
}
