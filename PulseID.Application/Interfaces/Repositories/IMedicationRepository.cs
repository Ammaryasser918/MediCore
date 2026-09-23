using PulseID.Application.Common.Interfaces;
using PulseID.Domain.Entities;

namespace PulseID.Application.Interfaces.Repositories;

public interface IMedicationRepository : IRepository<Medication>
{
    Task<IReadOnlyList<Medication>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}