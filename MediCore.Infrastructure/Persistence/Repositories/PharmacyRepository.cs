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
    public class PharmacyRepository : RepositoryBase<Pharmacy>, IPharmacyRepository
    {
        public PharmacyRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Pharmacy>> GetByStatusAsync(PharmacyStatus status, CancellationToken cancellationToken = default)
            => await _dbSet.Where(p => p.Status == status && !p.IsDeleted).ToListAsync(cancellationToken);

    }
}
