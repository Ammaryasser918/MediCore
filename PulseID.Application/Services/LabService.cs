using PulseID.Application.Common.Exceptions;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.Labs;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.Repositories;
using PulseID.Application.Interfaces.Services;
using PulseID.Domain.Entities;

namespace PulseID.Application.Services;

public class LabService : ILabService
{
    readonly IUnitOfWork _unitOfWork;
    readonly IIdentityService _identityService;

    public LabService(IUnitOfWork unitOfWork, IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
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

    public async Task RejectAsync(Guid id, Guid labAdminId, CancellationToken cancellationToken = default)
    {
        var lab = await _unitOfWork.Labs.GetByIdAsync(id, cancellationToken);

        if (lab is null)
            throw new NotFoundException($"Lab with id '{id}' was not found.");

        lab.Reject(labAdminId);

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

    public async Task HardDeleteAsync(Guid labId,  CancellationToken cancellationToken = default)
    {
        var lab = await _unitOfWork.Labs.GetByIdWithDeletedAsync(labId, cancellationToken);
        if (lab is null)
            throw new NotFoundException("Lab not found.");

        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var labTechs = await _unitOfWork.LabTechnicians.GetByLabAsync(labId, cancellationToken);
            foreach (var labTech in labTechs)
            {
                labTech.SoftDelete();
            }

            _unitOfWork.Labs.HardDelete(lab);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }


    }
    public async Task<IReadOnlyList<LabDto>> GetDeletedLabsAsync(CancellationToken cancellationToken = default)
    {
        var deletedLabs = await _unitOfWork.Labs.GetDeletedAsync(cancellationToken);

        return deletedLabs.Select(ToDto).ToList();
    }
    public async Task RestoreLabAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var lab = await _unitOfWork.Labs.GetDeletedByIdAsync(id, cancellationToken);

        if (lab is null)
            throw new NotFoundException("Deleted lab record not found.");

        _unitOfWork.Labs.Restore(lab);
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
            ApprovedByHospitalAdminId = lab.ApprovedByLabAdminId,
            ApprovedAt = lab.ApprovedAt,
            CreatedAt = lab.CreatedAt,
            UpdatedAt = lab.UpdatedAt
        };
    }
}