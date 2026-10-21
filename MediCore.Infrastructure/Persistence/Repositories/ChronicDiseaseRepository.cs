using Microsoft.EntityFrameworkCore;
using MediCore.Application.Interfaces.Repositories;
using MediCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Infrastructure.Persistence.Repositories
{
    public class ChronicDiseaseRepository : RepositoryBase<ChronicDisease>, IChronicDiseaseRepository
    {
        public ChronicDiseaseRepository(AppDbContext context) : base(context) { }

        public Task<ChronicDisease?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
            => _dbSet.FirstOrDefaultAsync(cd => cd.Name == name && !cd.IsDeleted, cancellationToken);

        public async Task<IReadOnlyList<ChronicDisease>> SearchByNameAsync(string term, CancellationToken cancellationToken = default)
            => await _dbSet.Where(cd => EF.Functions.Like(cd.Name, $"%{term}%") && !cd.IsDeleted).ToListAsync(cancellationToken);
    }
}
