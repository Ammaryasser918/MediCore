using PulseID.Application.Common.Interfaces;
using PulseID.Domain.Entities;
using PulseID.Domain.Enums;

namespace PulseID.Application.Interfaces.Repositories
{
    public interface IHospitalRepository : IRepository<Hospital>
    {
        Task<Hospital?> GetWithDepartmentsAsync(Guid hospitalId, CancellationToken cancellationToken = default);

        Task<Hospital?> GetWithDepartmentsAndDoctorsAsync(Guid hospitalId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Hospital>> GetByStatusAsync(HospitalStatus status, CancellationToken cancellationToken = default);
    }
}