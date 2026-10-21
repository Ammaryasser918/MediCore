using Microsoft.EntityFrameworkCore;
using MediCore.Application.Interfaces.Repositories;
using MediCore.Domain.Entities;
using MediCore.Domain.Enums;

namespace MediCore.Infrastructure.Persistence.Repositories
{
    public class HospitalRepository : RepositoryBase<Hospital>, IHospitalRepository
    {
        public HospitalRepository(AppDbContext context) : base(context) { }

        public Task<Hospital?> GetWithDepartmentsAsync(Guid hospitalId, CancellationToken cancellationToken = default)
            => _dbSet.Include(h => h.Departments.Where(d => !d.IsDeleted))
                .FirstOrDefaultAsync(h => h.Id == hospitalId && !h.IsDeleted, cancellationToken);

        public Task<Hospital?> GetWithDepartmentsAndDoctorsAsync(Guid hospitalId, CancellationToken cancellationToken = default)
            => _dbSet
                .Include(h => h.Departments.Where(d => !d.IsDeleted))
                .ThenInclude(d => d.Doctors.Where(doc => !doc.IsDeleted))
                .FirstOrDefaultAsync(h => h.Id == hospitalId && !h.IsDeleted, cancellationToken);

        public async Task<IReadOnlyList<Hospital>> GetByStatusAsync(HospitalStatus status, CancellationToken cancellationToken = default)
            => await _dbSet.Where(h => h.Status == status && !h.IsDeleted).ToListAsync(cancellationToken);
    }
}