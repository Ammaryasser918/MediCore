using PulseID.Application.Common.Exceptions;
using PulseID.Application.DTOs.Labs;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.Repositories;
using PulseID.Application.Interfaces.Services;
using PulseID.Domain.Entities;

namespace PulseID.Application.Services;

public class LabService : ILabService
{
    readonly IUnitOfWork _unitOfWork;

    public LabService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<LabDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var labs = await _unitOfWork.Labs.GetAllActiveAsync(cancellationToken);

        return labs.Select(ToDto).ToList();
    }

    public async Task<LabDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var lab = await _unitOfWork.Labs.GetByIdAsync(id, cancellationToken);

        return lab is null ? null : ToDto(lab);
    }

    public async Task<LabDto?> GetWithRequestsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var lab = await _unitOfWork.Labs.GetWithRequestsAsync(id, cancellationToken);

        return lab is null ? null : ToDto(lab);
    }

    public async Task<LabDto> CreateAsync(CreateLabDto dto, CancellationToken cancellationToken = default)
    {
        var lab = new Lab(
            dto.Name,
            dto.Address,
            dto.PhoneNumber,
            dto.LicenseNumber,
            dto.LicenseDocumentPath);

        await _unitOfWork.Labs.AddAsync(lab, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(lab);
    }

    public async Task<LabDto> UpdateAsync(Guid id, UpdateLabDto dto, CancellationToken cancellationToken = default)
    {
        var lab = await _unitOfWork.Labs.GetByIdAsync(id, cancellationToken);

        if (lab is null)
            throw new NotFoundException($"Lab with id '{id}' was not found.");

        lab.UpdateDetails(
            dto.Name,
            dto.Address,
            dto.PhoneNumber,
            dto.LicenseNumber);

        _unitOfWork.Labs.Update(lab);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(lab);
    }

    public async Task UpdateLicenseDocumentAsync(Guid id, string documentPath, CancellationToken cancellationToken = default)
    {
        var lab = await _unitOfWork.Labs.GetByIdAsync(id, cancellationToken);

        if (lab is null)
            throw new NotFoundException($"Lab with id '{id}' was not found.");

        lab.UpdateLicenseDocument(documentPath);

        _unitOfWork.Labs.Update(lab);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ApproveAsync(Guid id, Guid hospitalAdminId, CancellationToken cancellationToken = default)
    {
        var lab = await _unitOfWork.Labs.GetByIdAsync(id, cancellationToken);

        if (lab is null)
            throw new NotFoundException($"Lab with id '{id}' was not found.");

        lab.Approve(hospitalAdminId);

        _unitOfWork.Labs.Update(lab);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var lab = await _unitOfWork.Labs.GetByIdAsync(id, cancellationToken);

        if (lab is null)
            throw new NotFoundException($"Lab with id '{id}' was not found.");

        lab.Reject();

        _unitOfWork.Labs.Update(lab);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var lab = await _unitOfWork.Labs.GetByIdAsync(id, cancellationToken);

        if (lab is null)
            throw new NotFoundException($"Lab with id '{id}' was not found.");

        lab.Deactivate();

        _unitOfWork.Labs.Update(lab);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ReactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var lab = await _unitOfWork.Labs.GetByIdAsync(id, cancellationToken);

        if (lab is null)
            throw new NotFoundException($"Lab with id '{id}' was not found.");

        lab.Reactivate();

        _unitOfWork.Labs.Update(lab);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var lab = await _unitOfWork.Labs.GetByIdAsync(id, cancellationToken);

        if (lab is null)
            throw new NotFoundException($"Lab with id '{id}' was not found.");

        _unitOfWork.Labs.Delete(lab);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    static LabDto ToDto(Lab lab)
    {
        return new LabDto
        {
            Id = lab.Id,
            Name = lab.Name,
            Address = lab.Address,
            PhoneNumber = lab.PhoneNumber,
            LicenseNumber = lab.LicenseNumber,
            LicenseDocumentPath = lab.LicenseDocumentPath,
            Status = lab.Status,
            ApprovedByHospitalAdminId = lab.ApprovedByHospitalAdminId,
            ApprovedAt = lab.ApprovedAt,
            CreatedAt = lab.CreatedAt,
            UpdatedAt = lab.UpdatedAt
        };
    }
}