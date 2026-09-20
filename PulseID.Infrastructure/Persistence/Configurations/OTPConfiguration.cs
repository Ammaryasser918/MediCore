using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PulseID.Domain.Entities;
using PulseID.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Infrastructure.Persistence.Configurations
{
    public class OTPConfiguration : IEntityTypeConfiguration<OTP>
    {
        public void Configure(EntityTypeBuilder<OTP> builder)
        {
            builder.ToTable("OTPs");
            builder.HasKey(o => o.Id);

            builder.Property(o => o.CodeHash).IsRequired();
            builder.Property(o => o.Email).IsRequired();

            builder.HasIndex(o => new { o.Email, o.Purpose, o.IsUsed });

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(o => o.UserId).IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
