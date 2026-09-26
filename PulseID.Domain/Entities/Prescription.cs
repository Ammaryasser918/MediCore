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
    public class Prescription : BaseEntity
    {


        public Guid MedicalRecordId { get; private set; }
        public MedicalRecord MedicalRecord { get; private set; } = default!;

        public DateTime IssueDate { get; private set; }

        public DateTime? DispensedAt { get; private set; }
        public PrescriptionStatus Status { get; private set; } = PrescriptionStatus.Created;

        public string? Notes { get; private set; }

        readonly List<PrescriptionItem> _prescriptionItems = new List<PrescriptionItem>();
        public IReadOnlyCollection<PrescriptionItem> PrescriptionItems => _prescriptionItems.AsReadOnly();

        Prescription() { }

        public Prescription(Guid medicalRecordId, string? notes = null)
        {

            if (medicalRecordId == Guid.Empty)
                throw new ArgumentException("Medication Record ID is required", nameof(medicalRecordId));

            MedicalRecordId = medicalRecordId;
            IssueDate = DateTime.UtcNow;
            Notes = notes;
        }

        public void UpdateDetails(string? notes)
        {
            if (Status != PrescriptionStatus.Created)
                throw new BusinessRuleViolationException("Can't update a prescription after it is verified.");

            Notes = notes;
            MarkAsUpdated();
        }

        public PrescriptionItem AddItem(Guid medicationId, string dosage, int duration, string? instructions = null)
        {
            if (Status != PrescriptionStatus.Created)
                throw new BusinessRuleViolationException("Can't add medications after prescription is verified");

            var item = new PrescriptionItem(Id, medicationId, dosage, duration, instructions);
            _prescriptionItems.Add(item);
            MarkAsUpdated();
            return item;

        }

        public void UpdateItem(Guid itemId, Guid medicationId, string dosage, int duration, string? instructions = null)
        {
            if (Status != PrescriptionStatus.Created)
                throw new BusinessRuleViolationException("Can't update medications after prescription is verified.");

            var item = _prescriptionItems.FirstOrDefault(i => i.Id == itemId);

            if (item is null)
                throw new BusinessRuleViolationException("Prescription item was not found.");

            item.UpdateDetails(medicationId, dosage, duration, instructions);
            MarkAsUpdated();
        }

        public void RemoveItem(Guid itemId)
        {
            if (Status != PrescriptionStatus.Created)
            {
                throw new BusinessRuleViolationException("Can't remove medications after prescription is verified");
            }
            var item = _prescriptionItems.FirstOrDefault(i => i.Id == itemId);
            if (item is not null)
            {
                _prescriptionItems.Remove(item);
                MarkAsUpdated();
            }
        }

        public void MarkAsVerified()
        {
            if (!_prescriptionItems.Any())
            {
                throw new BusinessRuleViolationException("Can't verify an empty prescription");
            }
            Status = PrescriptionStatus.Verified;
            MarkAsUpdated();
        }

        public void RefreshStatus()
        {
            if (!_prescriptionItems.Any())
                return;

            if (_prescriptionItems.All(i => i.Status == PrescriptionItemStatus.Dispensed))
            {
                if (Status != PrescriptionStatus.Dispensed)
                {
                    Status = PrescriptionStatus.Dispensed;
                    DispensedAt = DateTime.UtcNow;
                }
            }
            else if (_prescriptionItems.Any(i => i.Status == PrescriptionItemStatus.Dispensed))
            {
                Status = PrescriptionStatus.PartiallyDispensed;
            }

            MarkAsUpdated();
        }


        public void Cancel()
        {
            if (Status == PrescriptionStatus.Dispensed || Status == PrescriptionStatus.PartiallyDispensed)
            {
                throw new BusinessRuleViolationException("The prescription can't be cancelled because it is dispensed or partially dispensed");
            }
            Status = PrescriptionStatus.Cancelled;
            MarkAsUpdated();
        }






    }
}
