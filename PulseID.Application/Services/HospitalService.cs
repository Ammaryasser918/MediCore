using PulseID.Application.Common.Exceptions;
using PulseID.Application.DTOs.Department;
using PulseID.Application.DTOs.Hospital;
using PulseID.Application.Interfaces.IServices;
using PulseID.Application.Interfaces;
using PulseID.Domain.Entities;

namespace PulseID.Application.Services;


public class HospitalService : IHospitalService
{
    readonly IUnitOfWork _unitOfWork;

    public HospitalService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    static DepartmentDto ToDepartmentDto(Department department) => new()
    {
        Id = department.Id,
        Name = department.Name,
        Description = department.Description,
        HospitalId = department.HospitalId,
        CreatedAt = department.CreatedAt,
        UpdatedAt = department.UpdatedAt
    };

    static HospitalDto ToDto(Hospital hospital) => new()
    {
        Id = hospital.Id,
        Name = hospital.Name,
        Address = hospital.Address,
        PhoneNumber = hospital.PhoneNumber,
        LicenseNumber = hospital.LicenseNumber,
        LicenseDocumentPath = hospital.LicenseDocumentPath,
        Status = hospital.Status,
        ApprovedByAdminId = hospital.ApprovedByAdminId,
        ApprovedAt = hospital.ApprovedAt,
        CreatedAt = hospital.CreatedAt,
        UpdatedAt = hospital.UpdatedAt,

        Departments = hospital.Departments
            .Select(ToDepartmentDto)
            .ToList()
    };

    public async Task<IReadOnlyList<HospitalDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var hospitals =
            await _unitOfWork.Hospitals.GetAllAsync(cancellationToken);

        return hospitals
            .Select(ToDto)
            .ToList();
    }
    public async Task<HospitalDto> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var hospital =
            await _unitOfWork.Hospitals.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Hospital), id);

        return ToDto(hospital);
    }
    public async Task<HospitalDto> CreateAsync(
        CreateHospitalDto hospitalDto,
        CancellationToken cancellationToken = default)
    {
        var hospital = new Hospital(
            hospitalDto.Name,
            hospitalDto.Address,
            hospitalDto.PhoneNumber,
            hospitalDto.LicenseNumber,
            hospitalDto.LicenseDocumentPath
        );

        await _unitOfWork.Hospitals.AddAsync(
            hospital,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ToDto(hospital);
    }
    public async Task<HospitalDto> UpdateAsync(
        Guid id,
        UpdateHospitalDto hospitalDto,
        CancellationToken cancellationToken = default)
    {
        var hospital =
            await _unitOfWork.Hospitals.GetByIdAsync(
                id,
                cancellationToken)
            ?? throw new NotFoundException(nameof(Hospital), id);

        hospital.UpdateDetails(
            hospitalDto.Name,
            hospitalDto.Address,
            hospitalDto.PhoneNumber,
            hospitalDto.LicenseNumber
        );


        _unitOfWork.Hospitals.Update(hospital);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ToDto(hospital);
    }
    public async Task<HospitalDto> UpdateLicenseDocumentAsync(
        Guid id,
        string NewLicenseDocumentPath,
        CancellationToken cancellationToken = default)
    {
        var hospital =
            await _unitOfWork.Hospitals.GetByIdAsync(
                id,
                cancellationToken)
            ?? throw new NotFoundException(nameof(Hospital), id);

        hospital.UpdateLicenseDocument(
            NewLicenseDocumentPath);

        _unitOfWork.Hospitals.Update(hospital);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ToDto(hospital);
    }
    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var hospital =
            await _unitOfWork.Hospitals.GetByIdAsync(
                id,
                cancellationToken)
            ?? throw new NotFoundException(nameof(Hospital), id);

        _unitOfWork.Hospitals.Delete(hospital);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    // Approval Work Flow
    public async Task<HospitalDto> ApproveAsync(
        Guid id,
        Guid adminId,
        CancellationToken cancellationToken = default)
    {
        var hospital =
            await _unitOfWork.Hospitals.GetByIdAsync(
                id,
                cancellationToken)
            ?? throw new NotFoundException(nameof(Hospital), id);

        hospital.Approve(adminId);

        _unitOfWork.Hospitals.Update(hospital);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ToDto(hospital);
    }
    public async Task<HospitalDto> RejectAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var hospital =
            await _unitOfWork.Hospitals.GetByIdAsync(
                id,
                cancellationToken)
            ?? throw new NotFoundException(nameof(Hospital), id);

        hospital.Reject();
 
        _unitOfWork.Hospitals.Update(hospital);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ToDto(hospital);
    }
    public async Task<HospitalDto> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var hospital =
            await _unitOfWork.Hospitals.GetByIdAsync(
                id,
                cancellationToken)
            ?? throw new NotFoundException(nameof(Hospital), id);

        hospital.Deactivate();

        _unitOfWork.Hospitals.Update(hospital);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ToDto(hospital);
    }
    public async Task<HospitalDto> ReactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var hospital =
            await _unitOfWork.Hospitals.GetByIdAsync(
                id,
                cancellationToken)
            ?? throw new NotFoundException(nameof(Hospital), id);

        hospital.Reactivate();

        _unitOfWork.Hospitals.Update(hospital);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ToDto(hospital);
    }
}
