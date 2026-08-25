using PulseID.Application.DTOs.MedicalFiles;
using PulseID.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.DTOs.MedicalRecord
{
    public class MedicalRecordDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid HospitalId { get; set; }
        public DateTime VisitDate { get; set; }
        public string Diagnosis { get; set; } = default!;
        public string? Symptoms { get; set; }
        public string? Notes { get; set; }
        public MedicalRecordStatus Status { get; set; }

        public Guid? PrescriptionId { get; set; }

        // Versioning
        public Guid? PreviousVersionId { get; set; }
        public Guid RootRecordId { get; set; }
        public bool IsLatestVersion { get; set; }

        public IReadOnlyList<MedicalFileDto> Files { get; set; } = new List<MedicalFileDto>();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
