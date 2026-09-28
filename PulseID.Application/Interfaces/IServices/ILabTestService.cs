using PulseID.Application.DTOs.Labs;

namespace PulseID.Application.Interfaces.Services;

public interface ILabTestService
{
    Task<IReadOnlyList<LabTestDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<LabTestDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<LabTestDto?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<LabTestDto> CreateAsync(CreateLabTestDto dto, CancellationToken cancellationToken = default);

    Task<LabTestDto> UpdateAsync(Guid id, UpdateLabTestDto dto, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task HardDeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LabTestDto>> GetDeletedLabTestsAsync(CancellationToken cancellationToken = default);

    Task RestoreLabTestAsync(Guid id, CancellationToken cancellationToken = default);

}