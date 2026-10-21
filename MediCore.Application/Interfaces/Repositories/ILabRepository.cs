using MediCore.Application.Common.Interfaces;
using MediCore.Domain.Entities;

namespace MediCore.Application.Interfaces.Repositories;

public interface ILabRepository : IRepository<Lab>
{
    Task<Lab?> GetWithRequestsAsync(Guid labId, CancellationToken cancellationToken = default);
}