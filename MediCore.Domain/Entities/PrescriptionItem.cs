using MediCore.Domain.Common;
using MediCore.Domain.Enums;
using MediCore.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Domain.Entities
{
    public class PrescriptionItem : BaseEntity
    {
        public Guid PrescriptionId { get; private set; }
        public Prescription Prescription { get; private set; } = default!;

        public Guid MedicationId { get; private set; }
        public Medication Medication { get; private set; } = default!;

        public string Dosage { get; private set; } = default!;
        public int Duration { get; private set; }
        public string? Instructions { get; private set; } = default!;


        public PrescriptionItemStatus Status { get; private set; } = PrescriptionItemStatus.Pending;

        public Guid? DispensedByUserId { get; private set; }
        public Pharmacist? DispensedByPharmacist { get; private set; }

        public DateTime? DispensedAt { get; private set; }

        PrescriptionItem() { }

        public PrescriptionItem(Guid prescriptionId, Guid medicationId, string dosage, int duration, string? instructions = null)
        {
            if (prescriptionId == Guid.Empty)
                throw new ArgumentException("Prescription ID is required.", nameof(prescriptionId));
            if (medicationId == Guid.Empty)
                throw new ArgumentException("Medication ID is required.", nameof(medicationId));
            if (string.IsNullOrWhiteSpace(dosage))
                throw new ArgumentException("Dosage is required.", nameof(dosage));
            if (duration <= 0)
                throw new ArgumentException("Duration must be greater than zero.", nameof(duration));

            PrescriptionId = prescriptionId;
            MedicationId = medicationId;
            Dosage = dosage;
            Duration = duration;
            Instructions = instructions;
        }

        public void UpdateDetails(Guid medicationId, string dosage, int duration, string? instructions = null)
        {
            if (Status != PrescriptionItemStatus.Pending)
                throw new BusinessRuleViolationException("Can only update a pending medication.");

            if (medicationId == Guid.Empty)
                throw new ArgumentException("Medication ID is required.", nameof(medicationId));

            if (Status == PrescriptionItemStatus.Dispensed)
                throw new BusinessRuleViolationException("Can't update a dispensed medication.");

            if (medicationId == Guid.Empty)
                throw new ArgumentException("Medication ID is required.", nameof(medicationId));

            if (string.IsNullOrWhiteSpace(dosage))
                throw new ArgumentException("Dosage is required.", nameof(dosage));

            if (duration <= 0)
                throw new ArgumentException("Duration must be greater than zero.", nameof(duration));

            MedicationId = medicationId;
            Dosage = dosage;
            Duration = duration;
            Instructions = instructions;

            MarkAsUpdated();
        }

        public void Dispense(Guid dispenserUserId)
        {
            if (dispenserUserId == Guid.Empty)
                throw new ArgumentException("Pharmacist Id is required.", nameof(dispenserUserId));

            if (Status == PrescriptionItemStatus.Dispensed)
                throw new BusinessRuleViolationException("This Medication is already dispensed");



            Status = PrescriptionItemStatus.Dispensed;
            DispensedByUserId = dispenserUserId;
            DispensedAt = DateTime.UtcNow;
            MarkAsUpdated();

        }







    }
}
