using System;

namespace PulseID.Application.DTOs.Department;

public class UpdateDepartmentDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
