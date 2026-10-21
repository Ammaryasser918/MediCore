using Microsoft.EntityFrameworkCore;
using MediCore.Application.Interfaces.Repositories;
using MediCore.Domain.Entities;

namespace MediCore.Infrastructure.Persistence.Repositories;

public class LabTestRepository : RepositoryBase<LabTest>, ILabTestRepository
{
    public LabTestRepository(AppDbContext context) : base(context)
    {
    }

    public Task<LabTest?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => _dbSet
            .FirstOrDefaultAsync(t => t.Name == name && !t.IsDeleted, cancellationToken);
}