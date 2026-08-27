using PulseID.Application.Common.Exceptions;
using PulseID.Application.DTOs.Department;
using PulseID.Application.DTOs.Doctor;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Entities;

namespace PulseID.Application.Services;

public class DepartmentService : IDepartmentService
{
    readonly IUnitOfWork _unitOfWork;

    public DepartmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    static DepartmentDto ToDto(Department department) => new()
    {
        Id = department.Id,
        Name = department.Name,
        Description = department.Description,
        HospitalId = department.HospitalId,
        CreatedAt = department.CreatedAt,
        UpdatedAt = department.UpdatedAt
    };

    static DoctorDto ToDoctorDto(Doctor doctor) => new()
    {
        Id = doctor.Id,
        UserId = doctor.UserId,
        FullName = doctor.FullName,
        SyndicateNumber = doctor.SyndicateNumber,
        SyndicateDocumentPath = doctor.SyndicateDocumentPath,
        NationalId = doctor.NationalId,
        Specialization = doctor.Specialization,
        PhoneNumber = doctor.PhoneNumber,
        PersonalImagePath = doctor.PersonalImagePath,
        Status = doctor.Status,
        DepartmentId = doctor.DepartmentId,
        DepartmentName = doctor.Department?.Name,
        ApprovedByHospitalAdminId = doctor.ApprovedByHospitalAdminId,
        ApprovedAt = doctor.ApprovedAt,
        CreatedAt = doctor.CreatedAt,
        UpdatedAt = doctor.UpdatedAt
    };

    public async Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var departments = await _unitOfWork.Departments.GetAllAsync(cancellationToken);

        return departments
            .Select(ToDto)
            .ToList();
    }

    public async Task<DepartmentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Department), id);

        return ToDto(department);
    }

    public async Task<DepartmentDto> CreateDepartmentAsync(Guid hospitalId, CreateDepartmentDto dto, CancellationToken cancellationToken = default)
    {
        var department = new Department(dto.Name, hospitalId, dto.Description);

        await _unitOfWork.Departments.AddAsync(department, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(department);
    }

    public async Task<DepartmentDto> UpdateAsync(Guid id, UpdateDepartmentDto dto, CancellationToken cancellationToken = default)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Department), id);

        department.EditDepartment(
            dto.Name,
            dto.Description);

        _unitOfWork.Departments.Update(department);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(department);
    }

    public async Task<IReadOnlyList<DoctorDto>> GetDoctorsAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(departmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Department), departmentId);

        var doctors = await _unitOfWork.Doctors.GetByDepartmentAsync(
            departmentId,
            cancellationToken);

        return doctors
            .Select(ToDoctorDto)
            .ToList();
    }

    public async Task<DepartmentDto> AssignDoctorToDepartmentAsync(Guid departmentId, Guid doctorId, CancellationToken cancellationToken = default)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(departmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Department), departmentId);

        var doctor = await _unitOfWork.Doctors.GetByIdAsync(doctorId, cancellationToken)
            ?? throw new NotFoundException(nameof(Doctor), doctorId);

        doctor.TransferToDepartment(departmentId);

        _unitOfWork.Doctors.Update(doctor);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(department);
    }

}