using Microsoft.EntityFrameworkCore;
using MediCore.Application.Interfaces.Repositories;
using MediCore.Domain.Entities;
using MediCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Infrastructure.Persistence.Repositories
{
    public class OTPRepository : RepositoryBase<OTP>, IOTPRepository
    {
        public OTPRepository(AppDbContext context) : base(context)
        {
        }

        public Task<OTP?> GetLatestActiveAsync(string email, OtpPurpose purpose, CancellationToken cancellationToken = default)
            => _dbSet
            .Where(o => o.Email == email && o.Purpose == purpose && o.ExpiresAt > DateTime.UtcNow && !o.IsUsed)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

    }
}
