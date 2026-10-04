using Microsoft.AspNetCore.Http;
using PulseID.Application.Common.Exceptions;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.Labs;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.Repositories;
using PulseID.Application.Interfaces.Services;
using PulseID.Domain.Entities;

namespace PulseID.Application.Services;

public class LabTechnicianService : ILabTechnicianService
{
    readonly IUnitOfWork _unitOfWork;
    readonly IIdentityService _identityService;
    readonly IFileStorageService _fileStorageService;

    public LabTechnicianService(IUnitOfWork unitOfWork, IIdentityService identityService, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _fileStorageService = fileStorageService;
    }

    public async Task<LabTechnicianDto> SetPersonalImageAsync(Guid id, IFormFile file, CancellationToken cancellationToken = default)
    {
        var labTech = await _unitOfWork.LabTechnicians.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(LabTechnician), id);
        
        var oldPath = labTech.PersonalImagePath;
        var newPath = await _fileStorageService.SaveAsync(file, UploadFolder.PersonalImages, cancellationToken);

        labTech.SetPersonalImage(newPath);
        _unitOfWork.LabTechnicians.Update(labTech);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _fileStorageService.DeleteAsync(oldPath, cancellationToken);

        var email = await _identityService.GetEmailByUserIdAsync(labTech.UserId, cancellationToken);
        return ToDto(labTech, email);

    }


    public  async Task<LabTechnicianDto> RemovePersonalImageAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var labTech = await _unitOfWork.LabTechnicians.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(LabTechnician), id);

        var oldPath = labTech.PersonalImagePath;

        labTech.RemovePersonalImage();

        await _fileStorageService.DeleteAsync(oldPath, cancellationToken);
        _unitOfWork.LabTechnicians.Update(labTech);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var email = await _identityService.GetEmailByUserIdAsync(labTech.UserId, cancellationToken);
        return ToDto(labTech, email);

    }


    public async Task<IReadOnlyList<LabTechnicianDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var technicians = await _unitOfWork.LabTechnicians.GetAllWithLabAsync(cancellationToken);
        var technicianDtos = new List<LabTechnicianDto>();
        foreach (var technician in  technicians)
        {
            var email = await _identityService.GetEmailByUserIdAsync(technician.UserId, cancellationToken);
            technicianDtos.Add(ToDto(technician, email));
        }
        return technicianDtos;
    }

    public async Task<LabTechnicianDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var technician = await _unitOfWork.LabTechnicians.GetWithLabAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(LabTechnician), id);
        var email = await _identityService.GetEmailByUserIdAsync(technician.UserId, cancellationToken);
        return technician is null ? null : ToDto(technician, email);
    }
    
    public async Task<LabTechnicianDto?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var technician = await _unitOfWork.LabTechnicians.GetByUserIdAsync(userId, cancellationToken) ?? throw new NotFoundException(nameof(LabTechnician), userId);
        var email = await _identityService.GetEmailByUserIdAsync(userId, cancellationToken);
        return technician is null ? null : ToDto(technician, email);
    }

    public async Task<IReadOnlyList<LabTechnicianDto>> GetByLabAsync(Guid labId, CancellationToken cancellationToken = default)
    {
        var technicians = await _unitOfWork.LabTechnicians.GetByLabAsync(labId, cancellationToken);
        var technicianDtos = new List<LabTechnicianDto>();
        foreach (var technician in technicians)
        {
            var email = await _identityService.GetEmailByUserIdAsync(technician.UserId, cancellationToken);
            technicianDtos.Add(ToDto(technician, email));
        }
        return technicianDtos;
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
            dto.DateOfBirth,
            dto.LabId
            );

        await _unitOfWork.LabTechnicians.AddAsync(technician, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var createdTechnician = await _unitOfWork.LabTechnicians.GetWithLabAsync(technician.Id, cancellationToken);

        if (createdTechnician is null)
            throw new NotFoundException($"Lab technician with id '{technician.Id}' was not found.");
        var email = await _identityService.GetEmailByUserIdAsync(createdTechnician.UserId);
        return ToDto(createdTechnician, email);
    }

    public async Task UpdateAsync(Guid id, UpdateLabTechnicianDto dto, CancellationToken cancellationToken = default)
    {
        var technician = await _unitOfWork.LabTechnicians.GetByIdAsync(id, cancellationToken);

        if (technician is null)
            throw new NotFoundException($"Lab technician with id '{id}' was not found.");

        technician.UpdatePersonalDetails(
            dto.FullName,
            dto.NationalId,
            dto.PhoneNumber,
            dto.DateOfBirth);

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
    
    public async Task ApproveAsync(Guid id, Guid labAdminId, CancellationToken cancellationToken = default)
    {
        var technician = await _unitOfWork.LabTechnicians.GetByIdAsync(id, cancellationToken);

        if (technician is null)
            throw new NotFoundException($"Lab technician with id '{id}' was not found.");

        technician.Approve(labAdminId);

        _unitOfWork.LabTechnicians.Update(technician);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectAsync(Guid id, Guid labAdminId, CancellationToken cancellationToken = default)
    {
        var technician = await _unitOfWork.LabTechnicians.GetByIdAsync(id, cancellationToken);

        if (technician is null)
            throw new NotFoundException($"Lab technician with id '{id}' was not found.");

        technician.Reject(labAdminId);

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

    public async Task HardDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var technician = await _unitOfWork.LabTechnicians.GetByIdWithDeletedAsync(id, cancellationToken);

        if (technician is null)
            throw new NotFoundException($"Lab technician with id '{id}' was not found.");
        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            _unitOfWork.LabTechnicians.HardDelete(technician);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var isDeleted = await _identityService.DeleteUserAsync(technician.UserId, cancellationToken);
            if (isDeleted)
                throw new Exception("Failed to delete user account.");

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        
    }

    public async Task<IReadOnlyList<LabTechnicianDto>> GetDeletedTechniciansAsync(CancellationToken cancellationToken = default)
    {
        var deletedTechnicians = await _unitOfWork.LabTechnicians.GetDeletedAsync(cancellationToken);
        var technicianDtos = new List<LabTechnicianDto>();
        foreach (var technician in deletedTechnicians)
        {
            var email = await _identityService.GetEmailByUserIdAsync(technician.UserId, cancellationToken);
            technicianDtos.Add(ToDto(technician, email));
        }
        return technicianDtos;
    }

    public async Task RestoreTechnicianAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var technician = await _unitOfWork.LabTechnicians.GetDeletedByIdAsync(id, cancellationToken);

        if (technician is null)
            throw new NotFoundException("Deleted lab technician record not found.");

        _unitOfWork.LabTechnicians.Restore(technician);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    LabTechnicianDto ToDto(LabTechnician technician, string email)
    {
        return new LabTechnicianDto
        {
            Id = technician.Id,
            Email = email,
            FullName = technician.FullName,
            NationalId = technician.NationalId,
            LicenseNumber = technician.LicenseNumber,
            LicenseDocumentPath = technician.LicenseDocumentPath,
            PhoneNumber = technician.PhoneNumber,
            PersonalImageUrl = _fileStorageService.GetUrl(technician.PersonalImagePath),
            Status = technician.Status,
            LabId = technician.LabId,
            LabName = technician.Lab?.Name,
            ApprovedByLabAdminId = technician.ApprovedByLabAdminId,
            ApprovedAt = technician.ApprovedAt,
            RejectedByLabAdminId = technician.RejectedByLabAdminId,
            RejectedAt = technician.RejectedAt,
            CreatedAt = technician.CreatedAt,
            UpdatedAt = technician.UpdatedAt
        };
    }
}