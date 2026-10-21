using System;

namespace MediCore.Application.DTOs.Hospital;

public class UpdateHospitalDto
{
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string? LicenseNumber { get; set; }
}
