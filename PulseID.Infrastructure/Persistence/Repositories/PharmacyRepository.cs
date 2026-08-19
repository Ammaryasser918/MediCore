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
    public class PharmacyRepository : RepositoryBase<Pharmacy>, IPharmacyRepository
    {
        public PharmacyRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Pharmacy>> GetByStatusAsync(PharmacyStatus status, CancellationToken cancellationToken = default)
            => await _dbSet.Where(p => p.Status == status && !p.IsDeleted).ToListAsync(cancellationToken);

    }
}
