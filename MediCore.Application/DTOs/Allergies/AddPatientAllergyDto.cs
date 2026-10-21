using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.DTOs.Allergies
{
    public class AddPatientAllergyDto
    {
        public Guid AllergyId { get; set; }
        public DateTime? DiagnosedDate { get; set; }
    }
}
