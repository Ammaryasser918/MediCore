using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Domain.Entities
{
    public class PatientAllergy
    {
        public Guid PatientId { get; private set; }
        public Guid AllergyId { get; private set; } 
        public Patient Patient { get; private set; } = default!;
        public Allergy Allergy { get; private set; } = default!;
        public DateTime? DiagnosedDate { get; private set; }
        

        PatientAllergy() { }

        public PatientAllergy(Guid patientId, Guid allergyId, DateTime? diagnosedDate = null)
        {
            if (patientId == Guid.Empty)
                throw new ArgumentException("Patient Id is required", nameof(patientId));
            if (allergyId == Guid.Empty)
                throw new ArgumentException("Allergy Id is required", nameof(allergyId));

            PatientId = patientId;
            AllergyId = allergyId;
            DiagnosedDate = diagnosedDate;
        }


    }
}
