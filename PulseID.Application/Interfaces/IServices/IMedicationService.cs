using PulseID.Application.DTOs.Medications;

namespace PulseID.Application.Interfaces.Services;

public interface IMedicationService
{
    Task<IReadOnlyList<MedicationDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<MedicationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MedicationDto> CreateAsync(CreateMedicationDto dto, CancellationToken cancellationToken = default);
    Task<MedicationDto> UpdateAsync(Guid id, UpdateMedicationDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}