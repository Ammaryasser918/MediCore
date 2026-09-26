using Microsoft.EntityFrameworkCore;
using PulseID.Application.Interfaces.Repositories;
using PulseID.Domain.Entities;

namespace PulseID.Infrastructure.Persistence.Repositories;

public class LabTestRepository : RepositoryBase<LabTest>, ILabTestRepository
{
    public LabTestRepository(AppDbContext context) : base(context)
    {
    }

    public Task<LabTest?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => _dbSet
            .FirstOrDefaultAsync(t => t.Name == name && !t.IsDeleted, cancellationToken);
}