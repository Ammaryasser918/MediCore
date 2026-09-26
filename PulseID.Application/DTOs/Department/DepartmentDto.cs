using System;
using PulseID.Application.DTOs.Doctor;

namespace PulseID.Application.DTOs.Department;

public class DepartmentDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public Guid HospitalId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public IReadOnlyList<DoctorDto> Doctors { get; set; } = new List<DoctorDto>();
}
