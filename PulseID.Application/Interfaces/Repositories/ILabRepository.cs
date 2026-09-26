using PulseID.Application.Common.Interfaces;
using PulseID.Domain.Entities;

namespace PulseID.Application.Interfaces.Repositories;

public interface ILabRepository : IRepository<Lab>
{
    Task<Lab?> GetWithRequestsAsync(Guid labId, CancellationToken cancellationToken = default);
}