using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Domain.Entities
{
    public class PatientChronicDisease
    {

        public Guid PatientId { get; private set; }
        public Guid ChronicDiseaseId { get; private set; }

        public Patient Patient { get; private set; } = default!;
        public ChronicDisease ChronicDisease { get; private set; } = default!;
        
        public DateTime? DiagnosedDate { get; private set; }

        PatientChronicDisease() { }

        public PatientChronicDisease(Guid patientId, Guid chronicDiseaseId, DateTime? diagnosedDate = null)
        {
            if (patientId == Guid.Empty)
                throw new ArgumentException("Patient Id is required.", nameof(patientId));
            if (chronicDiseaseId == Guid.Empty)
                throw new ArgumentException("Chronic Disease Id is required.", nameof(chronicDiseaseId));
            PatientId = patientId;
            ChronicDiseaseId = chronicDiseaseId;
            DiagnosedDate = diagnosedDate;
        }




    }
}
