using PulseID.Domain.Enums;

namespace PulseID.Application.DTOs.Prescriptions;

public class PrescriptionItemDto
{
    public Guid Id { get; set; }

    public Guid MedicationId { get; set; }

    public string MedicationName { get; set; } = default!;

    public string Dosage { get; set; } = default!;

    public int Duration { get; set; }

    public string? Instructions { get; set; }

    public PrescriptionItemStatus Status { get; set; }

    public Guid? DispensedByUserId { get; set; }

    public DateTime? DispensedAt { get; set; }
}

public class AddPrescriptionItemDto
{
    public Guid MedicationId { get; set; }

    public string Dosage { get; set; } = default!;

    public int Duration { get; set; }

    public string? Instructions { get; set; }
}

public class UpdatePrescriptionItemDto
{
    public Guid MedicationId { get; set; }

    public string Dosage { get; set; } = default!;

    public int Duration { get; set; }

    public string? Instructions { get; set; }
}