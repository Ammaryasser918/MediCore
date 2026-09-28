using PulseID.Application.Common.Interfaces;
using PulseID.Domain.Entities;

namespace PulseID.Application.Interfaces.Repositories;

public interface ILabTechnicianRepository : IRepository<LabTechnician>
{
    Task<IReadOnlyList<LabTechnician>> GetAllWithLabAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LabTechnician>> GetByLabAsync(Guid labId, CancellationToken cancellationToken = default);
    Task<LabTechnician?> GetWithLabAsync(Guid technicianId, CancellationToken cancellationToken = default);
    Task<LabTechnician?> GetByLicenseNumberAsync(string syndicateNumber, CancellationToken cancellationToken = default);
    Task<LabTechnician?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNationalIdAsync(string nationalid, CancellationToken cancellationToken = default);
    Task<LabTechnician?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}