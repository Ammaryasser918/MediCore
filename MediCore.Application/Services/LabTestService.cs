using MediCore.Application.Common.Exceptions;
using MediCore.Application.DTOs.Labs;
using MediCore.Application.Interfaces;
using MediCore.Application.Interfaces.Repositories;
using MediCore.Application.Interfaces.Services;
using MediCore.Domain.Entities;

namespace MediCore.Application.Services;

public class LabTestService : ILabTestService
{
    readonly IUnitOfWork _unitOfWork;

    public LabTestService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<LabTestDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tests = await _unitOfWork.LabTests.GetAllActiveAsync(cancellationToken);

        return tests.Select(ToDto).ToList();
    }

    public async Task<LabTestDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var test = await _unitOfWork.LabTests.GetByIdAsync(id, cancellationToken);

        return test is null ? null : ToDto(test);
    }

    public async Task<LabTestDto?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var test = await _unitOfWork.LabTests.GetByNameAsync(name, cancellationToken);

        return test is null ? null : ToDto(test);
    }

    public async Task<LabTestDto> CreateAsync(CreateLabTestDto dto, CancellationToken cancellationToken = default)
    {
        var test = new LabTest(
            dto.Name,
            dto.Type,
            dto.Description);

        await _unitOfWork.LabTests.AddAsync(test, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(test);
    }

    public async Task<LabTestDto> UpdateAsync(Guid id, UpdateLabTestDto dto, CancellationToken cancellationToken = default)
    {
        var test = await _unitOfWork.LabTests.GetByIdAsync(id, cancellationToken);

        if (test is null)
            throw new NotFoundException($"Lab test with id '{id}' was not found.");

        test.UpdateDetails(
            dto.Name,
            dto.Type,
            dto.Description);

        _unitOfWork.LabTests.Update(test);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(test);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var test = await _unitOfWork.LabTests.GetByIdAsync(id, cancellationToken);

        if (test is null)
            throw new NotFoundException($"Lab test with id '{id}' was not found.");

        _unitOfWork.LabTests.Delete(test);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task HardDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var test = await _unitOfWork.LabTests.GetByIdWithDeletedAsync(id, cancellationToken);

        if (test is null)
            throw new NotFoundException($"Lab test with id '{id}' was not found.");

        _unitOfWork.LabTests.HardDelete(test);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LabTestDto>> GetDeletedLabTestsAsync(CancellationToken cancellationToken = default)
    {
        var deletedTests = await _unitOfWork.LabTests.GetDeletedAsync(cancellationToken);

        return deletedTests.Select(ToDto).ToList();
    }

    public async Task RestoreLabTestAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var test = await _unitOfWork.LabTests.GetDeletedByIdAsync(id, cancellationToken);

        if (test is null)
            throw new NotFoundException("Deleted lab test record not found.");

        _unitOfWork.LabTests.Restore(test);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }


    static LabTestDto ToDto(LabTest test)
    {
        return new LabTestDto
        {
            Id = test.Id,
            Name = test.Name,
            Type = test.Type,
            Description = test.Description,
            CreatedAt = test.CreatedAt,
            UpdatedAt = test.UpdatedAt
        };
    }
}