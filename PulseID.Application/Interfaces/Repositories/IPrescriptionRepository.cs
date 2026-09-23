using PulseID.Application.Common.Interfaces;
using PulseID.Domain.Entities;
using PulseID.Domain.Enums;

namespace PulseID.Application.Interfaces.Repositories;

public interface IPrescriptionRepository : IRepository<Prescription>
{
    Task<Prescription?> GetWithItemsAsync(Guid prescriptionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Prescription>> GetByMedicalRecordAsync(Guid medicalRecordId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Prescription>> GetByStatusAsync(PrescriptionStatus status, CancellationToken cancellationToken = default);
}