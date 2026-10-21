using System;

namespace MediCore.Application.DTOs.Department;

public class CreateDepartmentDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
