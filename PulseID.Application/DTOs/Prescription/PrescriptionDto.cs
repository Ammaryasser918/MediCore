using PulseID.Domain.Enums;

namespace PulseID.Application.DTOs.Prescriptions;

public class PrescriptionDto
{
    public Guid Id { get; set; }

    public Guid MedicalRecordId { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }

    public DateTime IssueDate { get; set; }

    public DateTime? DispensedAt { get; set; }

    public PrescriptionStatus Status { get; set; }

    public string? Notes { get; set; }

    public List<PrescriptionItemDto> Items { get; set; } = [];

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

public class CreatePrescriptionDto
{
    public Guid MedicalRecordId { get; set; }

    public string? Notes { get; set; }
}

public class UpdatePrescriptionDto
{
    public string? Notes { get; set; }
}