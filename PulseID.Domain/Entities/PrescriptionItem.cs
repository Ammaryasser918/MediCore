using PulseID.Domain.Common;
using PulseID.Domain.Enums;
using PulseID.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Domain.Entities
{
    public class PrescriptionItem : BaseEntity
    {
        public Guid PrescriptionId { get; private set; }
        public Prescription Prescription { get; private set; } = default!;

        public string MedicationCode { get; private set; } = default!;
        public string MedicationName { get; private set; } = default!;

        public string Dosage { get; private set; } = default!;
        public int Duration { get; private set; }
        public string? Instructions { get; private set; } = default!;


        public PrescriptionItemStatus Status { get; private set; } = PrescriptionItemStatus.Pending;

        public Guid? DispensedByPharmacistId { get; private set; }
        public Pharmacist? DispensedbyPharmacist { get; private set; }

        public DateTime? DispensedAt { get; private set; }

        PrescriptionItem() { }

        public PrescriptionItem(Guid prescriptionId, string medicationCode, string medicationName, string dosage, int duration, string? instructions = null)
        {
            if (prescriptionId == Guid.Empty)
                throw new ArgumentException("Prescription ID is required.", nameof(prescriptionId));
            if (string.IsNullOrWhiteSpace(medicationCode))
                throw new ArgumentException("Medication Code is required.", nameof(medicationCode));
            if (string.IsNullOrWhiteSpace(medicationName))
                throw new ArgumentException("Medication Name is required.", nameof(medicationName));
            if (string.IsNullOrWhiteSpace(dosage))
                throw new ArgumentException("Dosage is required.", nameof(dosage));
            if (duration <= 0)
                throw new ArgumentException("Duration must be greater than zero.", nameof(duration));

            PrescriptionId = prescriptionId;
            MedicationCode = medicationCode;
            MedicationName = medicationName;
            Dosage = dosage;
            Duration = duration;
            Instructions = instructions;
        }


        public void Dispense(Guid pharmacistId)
        {
            if (Status == PrescriptionItemStatus.Dispensed)
                throw new BusinessRuleViolationException("This Medication is already dispensed");
            

            Status = PrescriptionItemStatus.Dispensed;
            DispensedByPharmacistId = pharmacistId;
            DispensedAt = DateTime.UtcNow;
            MarkAsUpdated();

        }

        





    }
}
