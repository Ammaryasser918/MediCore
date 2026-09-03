using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.DTOs.ChronicDiseases
{
    public class PatientChronicDiseaseDto
    {
        public Guid PatientId { get; set; }
        public Guid ChronicDiseaseId { get; set; }
        public string ChronicDiseaseName { get; set; } = default!;
        public string? ChronicDiseaseDescription { get; set; }
        public DateTime? DiagnosedDate { get; set; }
    }
}
