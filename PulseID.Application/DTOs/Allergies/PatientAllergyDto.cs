using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.DTOs.Allergies
{
    public class PatientAllergyDto
    {
        public Guid PatientId { get; set; }
        public Guid AllergyId { get; set; }
        public string AllergyName { get; set; } = default!;
        public string? AllergyDescription { get; set; }
        public DateTime? DiagnosedDate { get; set; }
    }
}
