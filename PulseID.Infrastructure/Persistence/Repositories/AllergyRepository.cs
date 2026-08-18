using Microsoft.EntityFrameworkCore;
using PulseID.Application.Interfaces.Repositories;
using PulseID.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Infrastructure.Persistence.Repositories
{
    public class AllergyRepository : RepositoryBase<Allergy>, IAllergyRepository
    {
        public AllergyRepository(AppDbContext context) : base(context) { }

        public Task<Allergy?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
            => _dbSet.FirstOrDefaultAsync(a => a.Name == name && !a.IsDeleted, cancellationToken);

        public async Task<IReadOnlyList<Allergy>> SearchByNameAsync(string term, CancellationToken cancellationToken = default)
            => await _dbSet.Where(a => EF.Functions.Like(a.Name, $"%{term}%") && !a.IsDeleted).ToListAsync(cancellationToken);
    }
}
