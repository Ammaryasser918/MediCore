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
    public class HospitalRepository : RepositoryBase<Hospital>, IHospitalRepository
    {

        public HospitalRepository(AppDbContext context) : base(context) { }

        public Task<Hospital?> GetWithDepartmentsAsync(Guid hospitalId, CancellationToken cancellationToken = default)
            => _dbSet.Include(h => h.Departments.Where(d => !d.IsDeleted))
            .FirstOrDefaultAsync(h => h.Id == hospitalId && !h.IsDeleted, cancellationToken);

        public async Task<IReadOnlyList<Hospital>> GetByStatusAsync(HospitalStatus status, CancellationToken cancellationToken = default)
            => await _dbSet.Where(h => h.Status == status && !h.IsDeleted).ToListAsync(cancellationToken);


    }
}
