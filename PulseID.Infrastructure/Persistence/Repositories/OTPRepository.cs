using Microsoft.EntityFrameworkCore;
using PulseID.Application.Interfaces.Repositories;
using PulseID.Domain.Entities;
using PulseID.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Infrastructure.Persistence.Repositories
{
    public class OTPRepository : RepositoryBase<OTP>, IOTPRepository
    {
        public OTPRepository(AppDbContext context) : base(context)
        {
        }

        public Task<OTP?> GetLatestActiveAsync(Guid userId, OtpPurpose purpose, CancellationToken cancellationToken = default)
            => _dbSet
            .Where(o => o.UserId == userId && o.Purpose == purpose && o.ExpiresAt > DateTime.UtcNow && !o.IsUsed)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

    }
}
