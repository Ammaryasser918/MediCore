using Microsoft.EntityFrameworkCore;
using MediCore.Application.Interfaces.Repositories;
using MediCore.Domain.Entities;

namespace MediCore.Infrastructure.Persistence.Repositories;

public class MedicationRepository : RepositoryBase<Medication>, IMedicationRepository
{
    public MedicationRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Medication>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(m => !m.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.Trim();

            query = query.Where(m =>
                m.CommercialNameEn.Contains(searchTerm) ||
                (m.CommercialNameAr != null && m.CommercialNameAr.Contains(searchTerm)) ||
                (m.ScientificName != null && m.ScientificName.Contains(searchTerm)));
        }

        return await query
            .OrderBy(m => m.CommercialNameEn)
            .Take(50)
            .ToListAsync(cancellationToken);
    }
}