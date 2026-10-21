using MediCore.Application.Common.Interfaces;
using MediCore.Domain.Entities;

namespace MediCore.Application.Interfaces.Repositories;

public interface ILabTestRepository : IRepository<LabTest>
{
    Task<LabTest?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}