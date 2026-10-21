using MediCore.Application.Common.Exceptions;
using MediCore.Application.DTOs.Department;
using MediCore.Application.DTOs.Hospital;
using MediCore.Application.DTOs.Doctor;
using MediCore.Application.Interfaces.IServices;
using MediCore.Application.Interfaces;
using MediCore.Domain.Entities;

namespace MediCore.Application.Services;


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

    DoctorDto ToDoctorDto(Doctor doctor) => new()
    {
        Id = doctor.Id,
        FullName = doctor.FullName,
        SyndicateNumber = doctor.SyndicateNumber,
        SyndicateDocumentPath = doctor.SyndicateDocumentPath,
        NationalId = doctor.NationalId,
        Specialization = doctor.Specialization,
        PhoneNumber = doctor.PhoneNumber,
        PersonalImageUrl = doctor.PersonalImagePath,
        Status = doctor.Status,
        DepartmentId = doctor.DepartmentId,
        DepartmentName = doctor.Department?.Name,
        ApprovedByHospitalAdminId = doctor.ApprovedByHospitalAdminId,
        ApprovedAt = doctor.ApprovedAt,
        CreatedAt = doctor.CreatedAt,
        UpdatedAt = doctor.UpdatedAt
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

    public async Task<IReadOnlyList<DepartmentDto>> GetDepartmentsAsync(Guid hospitalId, CancellationToken cancellationToken = default)
    {
        var hospital =
            await _unitOfWork.Hospitals.GetWithDepartmentsAsync(hospitalId, cancellationToken)
            ?? throw new NotFoundException(nameof(Hospital), hospitalId);

        return hospital.Departments
            .Select(ToDepartmentDto)
            .ToList();
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetDepartmentsWithDetailsAsync(Guid hospitalId, CancellationToken cancellationToken = default)
    {
        var hospital =
            await _unitOfWork.Hospitals.GetWithDepartmentsAndDoctorsAsync(hospitalId, cancellationToken)
            ?? throw new NotFoundException(nameof(Hospital), hospitalId);

        return hospital.Departments
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                HospitalId = d.HospitalId,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt,
                Doctors = d.Doctors
                    .Where(x => !x.IsDeleted)
                    .Select(ToDoctorDto)
                    .ToList()
            })
            .ToList();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var hospital =
            await _unitOfWork.Hospitals.GetWithDepartmentsAndDoctorsAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Hospital), id);

        _unitOfWork.Hospitals.Delete(hospital);

        foreach (var department in hospital.Departments.Where(d => !d.IsDeleted))
        {
            _unitOfWork.Departments.Delete(department);
            foreach (var doctor in department.Doctors.Where(d => !d.IsDeleted))
            {
                _unitOfWork.Doctors.Delete(doctor);
            }
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<HospitalDto> UpdateLicenseDocumentAsync(Guid id, string NewLicenseDocumentPath, CancellationToken cancellationToken = default)
    {
        var hospital =
            await _unitOfWork.Hospitals.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Hospital), id);

        hospital.UpdateLicenseDocument(NewLicenseDocumentPath);

        _unitOfWork.Hospitals.Update(hospital);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(hospital);
    }


    // Approval Work Flow
    public async Task<HospitalDto> ApproveAsync(Guid id, Guid adminId, CancellationToken cancellationToken = default)
    {
        var hospital =
            await _unitOfWork.Hospitals.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Hospital), id);

        hospital.Approve(adminId);

        _unitOfWork.Hospitals.Update(hospital);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(hospital);
    }
    public async Task<HospitalDto> RejectAsync(Guid id, Guid adminId, CancellationToken cancellationToken = default)
    {
        var hospital =
            await _unitOfWork.Hospitals.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Hospital), id);

        hospital.Reject(adminId);

        _unitOfWork.Hospitals.Update(hospital);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(hospital);
    }
    public async Task<HospitalDto> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var hospital =
            await _unitOfWork.Hospitals.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Hospital), id);

        hospital.Deactivate();

        _unitOfWork.Hospitals.Update(hospital);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(hospital);
    }
    public async Task<HospitalDto> ReactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var hospital =
            await _unitOfWork.Hospitals.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Hospital), id);

        hospital.Reactivate();

        _unitOfWork.Hospitals.Update(hospital);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(hospital);
    }

    public async Task HardDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var hospital = await _unitOfWork.Hospitals.GetByIdWithDeletedAsync(id, cancellationToken)
            ?? throw new NotFoundException("Hospital not found.");

        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var departments = await _unitOfWork.Departments.GetByHospitalIncludingDeletedAsync(id, cancellationToken);
            foreach (var department in departments)
            {
                var doctors = await _unitOfWork.Doctors.GetByDepartmentAsync(department.Id, cancellationToken);

                foreach (var doctor in doctors)
                {
                    doctor.DetachFromDepartment();
                    _unitOfWork.Doctors.Update(doctor);
                }
                _unitOfWork.Departments.HardDelete(department);
                _unitOfWork.Departments.Update(department);
            }
            _unitOfWork.Hospitals.HardDelete(hospital);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

    }

    public async Task<IReadOnlyList<HospitalDto>> GetDeletedAsync(CancellationToken cancellationToken = default)
    {
        var hospitals = await _unitOfWork.Hospitals.GetDeletedAsync(cancellationToken);
        return hospitals.Select(ToDto).ToList();
    }

    public async Task RestoreHospitalAsync(Guid hospitalId, CancellationToken cancellationToken = default)
    {
        var hospital = await _unitOfWork.Hospitals.GetByIdWithDeletedAsync(hospitalId, cancellationToken)
            ?? throw new NotFoundException($"Deleted hospital record not found.");
        _unitOfWork.Hospitals.Restore(hospital);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }


}
