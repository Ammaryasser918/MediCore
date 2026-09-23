using PulseID.Application.Common.Interfaces;
using PulseID.Domain.Entities;

namespace PulseID.Application.Interfaces.Repositories;

public interface ILabTestRepository : IRepository<LabTest>
{
    Task<LabTest?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}