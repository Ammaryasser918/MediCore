using MediCore.Domain.Enums;

namespace MediCore.Application.DTOs.Labs
{
    public class LabDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string? LicenseNumber { get; set; }
        public string? LicenseDocumentPath { get; set; }
        public LabStatus Status { get; set; }
        public Guid? ApprovedByHospitalAdminId { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateLabDto
    {
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string? LicenseNumber { get; set; }
        public string? LicenseDocumentPath { get; set; }
    }

    public class UpdateLabDto
    {
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string? LicenseNumber { get; set; }
    }
}