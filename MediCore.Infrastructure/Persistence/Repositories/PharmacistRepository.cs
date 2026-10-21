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
    public class PharmacistRepository : RepositoryBase<Pharmacist>, IPharmacistRepository
    {

        public PharmacistRepository(AppDbContext context) : base(context) { }

        public Task<Pharmacist?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => _dbSet.FirstOrDefaultAsync(ph => ph.UserId == userId && !ph.IsDeleted, cancellationToken);

        public Task<Pharmacist?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default)
            => _dbSet.FirstOrDefaultAsync(ph => ph.NationalId == nationalId && !ph.IsDeleted, cancellationToken);

        public Task<bool> ExistsByNationalIdAsync(string nationalid, CancellationToken cancellationToken = default)
            => _dbSet.AnyAsync(ph => ph.NationalId == nationalid && !ph.IsDeleted, cancellationToken);

        public Task<Pharmacist?> GetBySyndicateNumberAsync(string syndicateNumber, CancellationToken cancellationToken = default)
            => _dbSet.FirstOrDefaultAsync(ph => ph.SyndicateNumber == syndicateNumber && !ph.IsDeleted, cancellationToken);

        public async Task<IReadOnlyList<Pharmacist>> GetByStatusAsync(PharmacistStatus status, CancellationToken cancellationToken = default)
            => await _dbSet.Where(ph => ph.Status == status).ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<Pharmacist>> GetByPharmacyIdAsync(Guid pharmacyId, CancellationToken cancellationToken = default)
            => await _dbSet.Where(p => p.PharmacyId == pharmacyId && !p.IsDeleted).ToListAsync(cancellationToken);

    }
}
