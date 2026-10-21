using Microsoft.EntityFrameworkCore;
using MediCore.Application.Interfaces.Repositories;
using MediCore.Domain.Entities;

namespace MediCore.Infrastructure.Persistence.Repositories;

public class LabTechnicianRepository : RepositoryBase<LabTechnician>, ILabTechnicianRepository
{
    public LabTechnicianRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<LabTechnician>> GetAllWithLabAsync(CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(t => t.Lab)
            .Where(t => !t.IsDeleted)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LabTechnician>> GetByLabAsync(Guid labId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(t => t.Lab)
            .Where(t => t.LabId == labId && !t.IsDeleted)
            .ToListAsync(cancellationToken);

    public Task<LabTechnician?> GetWithLabAsync(Guid technicianId, CancellationToken cancellationToken = default)
        => _dbSet
            .Include(t => t.Lab)
            .FirstOrDefaultAsync(t => t.Id == technicianId && !t.IsDeleted, cancellationToken);

    public Task<LabTechnician?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => _dbSet.Include(t => t.Lab).FirstOrDefaultAsync(d => d.UserId == userId && !d.IsDeleted, cancellationToken);
    public Task<LabTechnician?> GetByLicenseNumberAsync(string LicenseNumber, CancellationToken cancellationToken = default)
            => _dbSet.FirstOrDefaultAsync(d => d.LicenseNumber == LicenseNumber && !d.IsDeleted, cancellationToken);

    public Task<LabTechnician?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default)
        => _dbSet.FirstOrDefaultAsync(d => d.NationalId == nationalId && !d.IsDeleted, cancellationToken);

    public Task<bool> ExistsByNationalIdAsync(string nationalid, CancellationToken cancellationToken = default)
        => _dbSet.AnyAsync(d => d.NationalId == nationalid && !d.IsDeleted, cancellationToken);
}