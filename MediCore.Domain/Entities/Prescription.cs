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
        IEnumerable<PrescriptionItem> ActiveItems => _prescriptionItems.Where(i => !i.IsDeleted);

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
            if (ActiveItems.Any(i => i.MedicationId == medicationId && i.Id != Guid.Empty))
                throw new BusinessRuleViolationException("This medication is already part of the prescription. Update the existing item instead of adding it twice.");

            var item = new PrescriptionItem(Id, medicationId, dosage, duration, instructions);
            _prescriptionItems.Add(item);
            MarkAsUpdated();
            return item;

        }

        public void UpdateItem(Guid itemId, Guid medicationId, string dosage, int duration, string? instructions = null)
        {
            if (Status != PrescriptionStatus.Created)
                throw new BusinessRuleViolationException("Can't update medications after prescription is verified.");

            var item = _prescriptionItems.FirstOrDefault(i => i.Id == itemId && !i.IsDeleted);

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

            if (Status == PrescriptionStatus.Cancelled || Status == PrescriptionStatus.Expired)
                return;
            var relevantItems = ActiveItems.Where(i => i.Status != PrescriptionItemStatus.Rejected).ToList();

            if (!relevantItems.Any())
                return;

            if (relevantItems.All(i => i.Status == PrescriptionItemStatus.Dispensed))
            {
                if (Status != PrescriptionStatus.Dispensed)
                {
                    Status = PrescriptionStatus.Dispensed;
                    DispensedAt = DateTime.UtcNow;
                }
            }
            else if (relevantItems.Any(i => i.Status == PrescriptionItemStatus.Dispensed))
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

            if (Status == PrescriptionStatus.Cancelled)
                throw new BusinessRuleViolationException("The prescription is already cancelled.");

            if (Status == PrescriptionStatus.Expired)
                throw new BusinessRuleViolationException("An expired prescription can't be cancelled.");

            Status = PrescriptionStatus.Cancelled;
            MarkAsUpdated();
        }






    }
}
