using PulseID.Application.Common.Exceptions;
using PulseID.Application.DTOs.Labs;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.Repositories;
using PulseID.Application.Interfaces.Services;
using PulseID.Domain.Entities;

namespace PulseID.Application.Services;

public class LabTechnicianService : ILabTechnicianService
{
    readonly IUnitOfWork _unitOfWork;

    public LabTechnicianService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<LabTechnicianDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var technicians = await _unitOfWork.LabTechnicians.GetAllWithLabAsync(cancellationToken);

        return technicians.Select(ToDto).ToList();
    }

    public async Task<LabTechnicianDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var technician = await _unitOfWork.LabTechnicians.GetWithLabAsync(id, cancellationToken);
        return technician is null ? null : ToDto(technician);
    }

    public async Task<IReadOnlyList<LabTechnicianDto>> GetByLabAsync(Guid labId, CancellationToken cancellationToken = default)
    {
        var technicians = await _unitOfWork.LabTechnicians.GetByLabAsync(labId, cancellationToken);

        return technicians.Select(ToDto).ToList();
    }

    public async Task<LabTechnicianDto> CreateAsync(CreateLabTechnicianDto dto, CancellationToken cancellationToken = default)
    {
        var technician = new LabTechnician(
            dto.UserId,
            dto.FullName,
            dto.NationalId,
            dto.LicenseNumber,
            dto.LicenseDocumentPath,
            dto.PhoneNumber,
            dto.LabId,
            dto.PersonalImagePath);

        await _unitOfWork.LabTechnicians.AddAsync(technician, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var createdTechnician = await _unitOfWork.LabTechnicians.GetWithLabAsync(technician.Id, cancellationToken);

        if (createdTechnician is null)
            throw new NotFoundException($"Lab technician with id '{technician.Id}' was not found.");

        return ToDto(createdTechnician);
    }

    public async Task UpdateAsync(Guid id, UpdateLabTechnicianDto dto, CancellationToken cancellationToken = default)
    {
        var technician = await _unitOfWork.LabTechnicians.GetByIdAsync(id, cancellationToken);

        if (technician is null)
            throw new NotFoundException($"Lab technician with id '{id}' was not found.");

        technician.UpdatePersonalDetails(
            dto.PhoneNumber,
            dto.PersonalImagePath);

        _unitOfWork.LabTechnicians.Update(technician);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task TransferToLabAsync(Guid id, Guid labId, CancellationToken cancellationToken = default)
    {
        var technician = await _unitOfWork.LabTechnicians.GetByIdAsync(id, cancellationToken);

        if (technician is null)
            throw new NotFoundException($"Lab technician with id '{id}' was not found.");

        technician.TransferToLab(labId);

        _unitOfWork.LabTechnicians.Update(technician);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ApproveAsync(Guid id, Guid hospitalAdminId, CancellationToken cancellationToken = default)
    {
        var technician = await _unitOfWork.LabTechnicians.GetByIdAsync(id, cancellationToken);

        if (technician is null)
            throw new NotFoundException($"Lab technician with id '{id}' was not found.");

        technician.Approve(hospitalAdminId);

        _unitOfWork.LabTechnicians.Update(technician);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var technician = await _unitOfWork.LabTechnicians.GetByIdAsync(id, cancellationToken);

        if (technician is null)
            throw new NotFoundException($"Lab technician with id '{id}' was not found.");

        technician.Reject();

        _unitOfWork.LabTechnicians.Update(technician);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var technician = await _unitOfWork.LabTechnicians.GetByIdAsync(id, cancellationToken);

        if (technician is null)
            throw new NotFoundException($"Lab technician with id '{id}' was not found.");

        technician.Deactivate();

        _unitOfWork.LabTechnicians.Update(technician);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ReactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var technician = await _unitOfWork.LabTechnicians.GetByIdAsync(id, cancellationToken);

        if (technician is null)
            throw new NotFoundException($"Lab technician with id '{id}' was not found.");

        technician.Reactivate();

        _unitOfWork.LabTechnicians.Update(technician);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var technician = await _unitOfWork.LabTechnicians.GetByIdAsync(id, cancellationToken);

        if (technician is null)
            throw new NotFoundException($"Lab technician with id '{id}' was not found.");

        _unitOfWork.LabTechnicians.Delete(technician);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    static LabTechnicianDto ToDto(LabTechnician technician)
    {
        return new LabTechnicianDto
        {
            Id = technician.Id,
            UserId = technician.UserId,
            FullName = technician.FullName,
            NationalId = technician.NationalId,
            LicenseNumber = technician.LicenseNumber,
            LicenseDocumentPath = technician.LicenseDocumentPath,
            PhoneNumber = technician.PhoneNumber,
            PersonalImagePath = technician.PersonalImagePath,
            Status = technician.Status,
            LabId = technician.LabId,
            LabName = technician.Lab.Name,
            ApprovedByHospitalAdminId = technician.ApprovedByHospitalAdminId,
            ApprovedAt = technician.ApprovedAt,
            CreatedAt = technician.CreatedAt,
            UpdatedAt = technician.UpdatedAt
        };
    }
}