using MediCore.Application.Common.Interfaces;
using MediCore.Domain.Entities;
using MediCore.Domain.Enums;

namespace MediCore.Application.Interfaces.Repositories;

public interface IPrescriptionRepository : IRepository<Prescription>
{
    Task<Prescription?> GetWithItemsAsync(Guid prescriptionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Prescription>> GetByMedicalRecordAsync(Guid medicalRecordId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Prescription>> GetByStatusAsync(PrescriptionStatus status, CancellationToken cancellationToken = default);
}