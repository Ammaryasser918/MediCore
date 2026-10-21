using MediCore.Domain.Enums;

namespace MediCore.Application.DTOs.Labs
{
    public class LabTestRequestDto
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = default!;

        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = default!;

        public Guid LabTestId { get; set; }
        public string LabTestName { get; set; } = default!;

        public Guid? LabId { get; set; }
        public string? LabName { get; set; }

        public LabTestRequestStatus Status { get; set; }

        public string? Notes { get; set; }

        public string? ResultFileUrl { get; set; }

        public DateTime RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? SampleCollectedAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateLabTestRequestDto
    {
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid LabTestId { get; set; }
        public Guid? LabId { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateLabTestRequestDto
    {
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid LabTestId { get; set; }
        public Guid? LabId { get; set; }
        public string? Notes { get; set; }
    }
}