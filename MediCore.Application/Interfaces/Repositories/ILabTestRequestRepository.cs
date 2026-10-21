using MediCore.Application.Common.Interfaces;
using MediCore.Domain.Entities;
using MediCore.Domain.Enums;

namespace MediCore.Application.Interfaces.Repositories;

public interface ILabTestRequestRepository : IRepository<LabTestRequest>
{
    Task<IReadOnlyList<LabTestRequest>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LabTestRequest>> GetByPatientAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LabTestRequest>> GetByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LabTestRequest>> GetByLabAsync(Guid labId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LabTestRequest>> GetByStatusAsync(LabTestRequestStatus status, CancellationToken cancellationToken = default);
    Task<LabTestRequest?> GetWithDetailsAsync(Guid requestId, CancellationToken cancellationToken = default);
}