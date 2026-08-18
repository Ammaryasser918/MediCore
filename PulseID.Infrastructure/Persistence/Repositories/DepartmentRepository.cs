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
    public class DepartmentRepository : RepositoryBase<Department>, IDepartmentRepository
    {
        public DepartmentRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Department>> GetByHospitalAsync(Guid hospitalId, CancellationToken cancellationToken = default)
            => await _dbSet.Where(d => d.HospitalId == hospitalId && !d.IsDeleted).ToListAsync(cancellationToken);

        public Task<bool> ExistsByNameInHospitalAsync(Guid hospitalId, string name, CancellationToken cancellationToken = default)
            => _dbSet.AnyAsync(d => d.HospitalId == hospitalId && d.Name == name && !d.IsDeleted, cancellationToken);

    }
}
