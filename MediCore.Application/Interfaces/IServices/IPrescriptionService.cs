using MediCore.Application.DTOs.Prescriptions;
using MediCore.Domain.Enums;

namespace MediCore.Application.Interfaces.Services;

public interface IPrescriptionService
{
    Task<IReadOnlyList<PrescriptionDto>> GetByMedicalRecordAsync(Guid medicalRecordId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PrescriptionDto>> GetByStatusAsync(PrescriptionStatus status, CancellationToken cancellationToken = default);

    Task<PrescriptionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PrescriptionDto?> GetWithItemsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PrescriptionDto> CreateAsync(CreatePrescriptionDto dto, CancellationToken cancellationToken = default);

    Task<PrescriptionDto> UpdateAsync(Guid id, UpdatePrescriptionDto dto, CancellationToken cancellationToken = default);

    Task AddItemAsync(Guid prescriptionId, AddPrescriptionItemDto dto, CancellationToken cancellationToken = default);

    Task UpdateItemAsync(Guid prescriptionId, Guid itemId, UpdatePrescriptionItemDto dto, CancellationToken cancellationToken = default);

    Task RemoveItemAsync(Guid prescriptionId, Guid itemId, CancellationToken cancellationToken = default);

    Task VerifyAsync(Guid prescriptionId, CancellationToken cancellationToken = default);

    Task DispenseItemAsync(Guid prescriptionId, Guid itemId, Guid pharmacistId, CancellationToken cancellationToken = default);

    Task CancelAsync(Guid prescriptionId, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid prescriptionId, CancellationToken cancellationToken = default);

    Task HardDeleteAsync(Guid prescriptionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PrescriptionDto>> GetDeletedAsync(CancellationToken cancellationToken = default);

    Task RestorePrescriptionAsync(Guid prescriptionId, CancellationToken cancellationToken = default);

}