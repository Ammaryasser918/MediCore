using PulseID.Domain.Enums;

namespace PulseID.Application.DTOs.Labs;

public class LabTechnicianDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string FullName { get; set; } = default!;
    public string NationalId { get; set; } = default!;

    public string LicenseNumber { get; set; } = default!;
    public string? LicenseDocumentPath { get; set; }

    public string PhoneNumber { get; set; } = default!;
    public string? PersonalImagePath { get; set; }

    public LabTechnicianStatus Status { get; set; }

    public Guid LabId { get; set; }
    public string LabName { get; set; } = default!;

    public Guid? ApprovedByHospitalAdminId { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateLabTechnicianDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = default!;
    public string NationalId { get; set; } = default!;
    public string LicenseNumber { get; set; } = default!;
    public string? LicenseDocumentPath { get; set; }
    public string PhoneNumber { get; set; } = default!;
    public Guid LabId { get; set; }
    public string? PersonalImagePath { get; set; }
}

public class UpdateLabTechnicianDto
{
    public string PhoneNumber { get; set; } = default!;
    public string? PersonalImagePath { get; set; }
}