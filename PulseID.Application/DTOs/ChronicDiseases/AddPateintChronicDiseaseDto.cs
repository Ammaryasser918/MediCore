using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.DTOs.ChronicDiseases
{
    public class AddPateintChronicDiseaseDto
    {
        public Guid ChronicDiseaseId { get; set; }
        public DateTime? DiagnosedDate { get; set; }
    }
}
