using Microsoft.EntityFrameworkCore;
using PulseID.Application.Interfaces.Repositories;
using PulseID.Domain.Entities;

namespace PulseID.Infrastructure.Persistence.Repositories
{
    public class DepartmentRepository : RepositoryBase<Department>, IDepartmentRepository
    {
        public DepartmentRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Department>> GetByHospitalAsync(Guid hospitalId, CancellationToken cancellationToken = default)
            => await _dbSet.Where(d => d.HospitalId == hospitalId && !d.IsDeleted).ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<Department>> GetByHospitalIncludingDeletedAsync(Guid hospitalId, CancellationToken cancellationToken = default)
            => await _dbSet.Where(d => d.HospitalId == hospitalId).ToListAsync(cancellationToken);

        public async Task<Department?> GetWithDoctorsAsync(Guid departmentId, CancellationToken cancellationToken = default)
            => await _dbSet.Include(d => d.Doctors).FirstOrDefaultAsync(d => d.Id == departmentId && !d.IsDeleted, cancellationToken);

        public Task<bool> ExistsByNameInHospitalAsync(Guid hospitalId, string name, CancellationToken cancellationToken = default)
            => _dbSet.AnyAsync(d => d.HospitalId == hospitalId && d.Name == name && !d.IsDeleted, cancellationToken);
    }
}