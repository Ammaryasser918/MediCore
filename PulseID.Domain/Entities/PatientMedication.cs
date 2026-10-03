using PulseID.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Domain.Entities
{
    public class PatientMedication : BaseEntity
    {
        public Guid PatientId { get; private set; }
        public Patient Patient { get; private set; } = default!;

        public Guid MedicationId { get; private set; }
        public Medication Medication { get; private set; } = default!;

        public string? Dosage { get; private set; }
        public string? Frequency { get; private set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public bool IsActive { get; private set; }

        PatientMedication() { }

        public PatientMedication(Guid patientId, Guid medicationId, string? dosage = null,
            string? frequency = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (patientId == Guid.Empty)
                throw new ArgumentException("Patient Id is required.", nameof(patientId));
            if (medicationId == Guid.Empty)
                throw new ArgumentException("Medication Id is required.", nameof(medicationId));

            PatientId = patientId;
            MedicationId = medicationId;
            Dosage = dosage;
            Frequency = frequency;
            StartDate = startDate;
            EndDate = endDate;
            IsActive = true;
        }

        public void Stop(DateTime? endDate = null)
        {
            IsActive = false;
            EndDate = endDate ?? DateTime.UtcNow;
            MarkAsUpdated();
        }
    }
}
