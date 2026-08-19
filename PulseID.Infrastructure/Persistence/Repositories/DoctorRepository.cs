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
    public class DoctorRepository : RepositoryBase<Doctor>, IDoctorRepository
    {
        public DoctorRepository(AppDbContext context) : base(context) { }

        public Task<Doctor?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => _dbSet.FirstOrDefaultAsync(d => d.UserId == userId && !d.IsDeleted, cancellationToken);

        
        public Task<Doctor?> GetBySyndicateNumberAsync(string syndicateNumber, CancellationToken cancellationToken = default)
            => _dbSet.FirstOrDefaultAsync(d => d.SyndicateNumber == syndicateNumber && !d.IsDeleted, cancellationToken);

        public Task<Doctor?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default)
            => _dbSet.FirstOrDefaultAsync(d => d.NationalId == nationalId && !d.IsDeleted, cancellationToken);

        public async Task<IReadOnlyList<Doctor>> GetByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default)
            => await _dbSet.Where(d => d.DepartmentId == departmentId).ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<Doctor>> GetByStatusAsync(DoctorStatus status, CancellationToken cancellationToken = default)
            => await _dbSet.Where(d => d.Status == status && !d.IsDeleted).ToListAsync(cancellationToken);

        public async Task<Doctor?> GetWithDepartmentAsync(Guid doctorId, CancellationToken cancellationToken = default)
            => await _dbSet.Include(d => d.Department)
                     .FirstOrDefaultAsync(d => d.Id == doctorId && !d.IsDeleted, cancellationToken);

    }
}
