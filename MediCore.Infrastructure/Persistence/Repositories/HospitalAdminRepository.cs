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
    public class HospitalAdminRepository : RepositoryBase<HospitalAdmin>, IHospitalAdminRepository
    {
        public HospitalAdminRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<HospitalAdmin?> GetByUserIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(ha => ha.Hospital)
                .FirstOrDefaultAsync(ha => ha.Id == id && !ha.IsDeleted, cancellationToken);
        }


    }
}
