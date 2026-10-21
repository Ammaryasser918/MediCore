using System;
using MediCore.Domain.Enums;
using MediCore.Application.DTOs.Department;

namespace MediCore.Application.DTOs.Hospital;

public class HospitalDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;

    public string? LicenseNumber { get; set; }
    public string? LicenseDocumentPath { get; set; }

    public HospitalStatus Status { get; set; }

    public Guid? ApprovedByAdminId { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public IReadOnlyList<DepartmentDto> Departments { get; set; } = new List<DepartmentDto>();
}

