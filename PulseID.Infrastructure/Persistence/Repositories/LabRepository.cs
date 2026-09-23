using Microsoft.EntityFrameworkCore;
using PulseID.Application.Interfaces.Repositories;
using PulseID.Domain.Entities;

namespace PulseID.Infrastructure.Persistence.Repositories;

public class LabRepository : RepositoryBase<Lab>, ILabRepository
{
    public LabRepository(AppDbContext context) : base(context)
    {
    }

    public Task<Lab?> GetWithRequestsAsync(Guid labId, CancellationToken cancellationToken = default)
        => _dbSet
            .Include(l => l.TestRequests.Where(r => !r.IsDeleted))
            .FirstOrDefaultAsync(l => l.Id == labId && !l.IsDeleted, cancellationToken);
}