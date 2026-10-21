using MediCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.DTOs.MedicalRecord
{
    public class MedicalRecordVersionDto
    {
        public Guid Id { get; set; }
        public Guid? PreviousVersionId { get; set; }
        public DateTime VisitDate { get; set; } = default!;
        public string Diagnosis { get; set; } = default!;
        public string? Symptoms { get; set; }
        public string? Notes { get; set; }
        public MedicalRecordStatus Status { get; set; }
        public bool IsLatestVersion { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
