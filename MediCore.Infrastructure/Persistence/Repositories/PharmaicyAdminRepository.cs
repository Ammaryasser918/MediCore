using Microsoft.EntityFrameworkCore;
using MediCore.Application.Interfaces.Repositories;
using MediCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Infrastructure.Persistence.Repositories
{
    public class PharmaicyAdminRepository : RepositoryBase<PharmacyAdmin>, IPharmacyAdminRepository
    {
        public PharmaicyAdminRepository(AppDbContext context) : base(context) { }

        public async Task<PharmacyAdmin?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(pa => pa.Pharmacy)
                .FirstOrDefaultAsync(pa => pa.UserId == userId && !pa.IsDeleted, cancellationToken);
        }
    }
}
