using Microsoft.EntityFrameworkCore;
using PulseID.Application.Interfaces.Repositories;
using PulseID.Domain.Entities;

namespace PulseID.Infrastructure.Persistence.Repositories;

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
}