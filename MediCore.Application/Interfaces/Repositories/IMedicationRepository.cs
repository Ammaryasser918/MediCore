using MediCore.Application.Common.Interfaces;
using MediCore.Domain.Entities;

namespace MediCore.Application.Interfaces.Repositories;

public interface IMedicationRepository : IRepository<Medication>
{
    Task<IReadOnlyList<Medication>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}