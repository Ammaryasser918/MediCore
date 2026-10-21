using Microsoft.EntityFrameworkCore;
using MediCore.Application.Interfaces.Repositories;
using MediCore.Domain.Entities;

namespace MediCore.Infrastructure.Persistence.Repositories;

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