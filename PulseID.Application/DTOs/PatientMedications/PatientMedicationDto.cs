using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.DTOs.PatientMedications
{
    public class PatientMedicationDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid MedicationId { get; set; }
        public string MedicationName { get; set; } = default!;
        public string? MedicationNameAr { get; set; }
        public string? ScientificName { get; set; }
        public string? Dosage { get; set; }
        public string? Frequency { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
