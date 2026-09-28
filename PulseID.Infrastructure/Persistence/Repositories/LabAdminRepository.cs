using Microsoft.EntityFrameworkCore;
using PulseID.Application.Interfaces.Repositories;
using PulseID.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Infrastructure.Persistence.Repositories
{
    public class LabAdminRepository : RepositoryBase<LabAdmin>, ILabAdminRepository
    {
        public LabAdminRepository(AppDbContext context) : base(context)
        {
            
        }


        public async Task<LabAdmin?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Include(l => l.Lab).FirstOrDefaultAsync(pa => pa.UserId == userId && !pa.IsDeleted, cancellationToken);
        }


    }
}
