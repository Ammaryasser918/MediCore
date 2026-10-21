using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.DTOs.MedicalRecord
{
    public class CreateMedicalRecordDto
    {
        public Guid PatientId { get; set; }
        //public Guid HospitalId { get; set; }
        public DateTime VisitDate { get; set; }
        public string Diagnosis { get; set; } = default!;
        public string? Symptoms { get; set; }
        public string? Notes { get; set; }

    }
}
