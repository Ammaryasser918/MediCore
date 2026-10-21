using MediCore.Application.Common.Interfaces;
using MediCore.Domain.Entities;
using MediCore.Domain.Enums;

namespace MediCore.Application.Interfaces.Repositories
{
    public interface IHospitalRepository : IRepository<Hospital>
    {
        Task<Hospital?> GetWithDepartmentsAsync(Guid hospitalId, CancellationToken cancellationToken = default);

        Task<Hospital?> GetWithDepartmentsAndDoctorsAsync(Guid hospitalId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Hospital>> GetByStatusAsync(HospitalStatus status, CancellationToken cancellationToken = default);
    }
}