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
    public class Pharmacist : BaseEntity
    {
        public Guid UserId { get; private set; } = default!;
        public string FullName { get; private set; } = default!;
        public string SyndicateNumber { get; private set; } = default!;
        public string SyndicateDocumentPath { get; private set; } = default!;
        public string NationalId { get; private set; } = default!;
        public PharmacistStatus Status { get; private set; }
        public string PhoneNumber { get; private set; } = default!;
        public Guid? PharmacyId { get; private set; }
        public Pharmacy Pharmacy { get; private set; } = default!;
        public Guid? ApprovedByAdminId { get; private set; }
        public DateTime? ApprovedAt { get; private set; }


        readonly List<PrescriptionItem> _dispensedItems = new List<PrescriptionItem>();
        public IReadOnlyCollection<PrescriptionItem> PrescriptionItems => _dispensedItems.AsReadOnly();

        Pharmacist() { }

        public Pharmacist(Guid userId, string fullName, string syndicateNumber, string syndicateDocumentPath, 
            string nationalId, string phoneNumber, Guid? pharmacyId = null)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User Id is required", nameof(userId));

            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name is required.", nameof(fullName));

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number is required", nameof(phoneNumber));
            if (string.IsNullOrWhiteSpace(nationalId))
                throw new ArgumentException(nationalId);

            UserId = userId;
            FullName = fullName;
            SyndicateNumber = syndicateNumber;
            SyndicateDocumentPath = syndicateDocumentPath;
            PhoneNumber = phoneNumber;
            PharmacyId = pharmacyId;
            NationalId = nationalId;
            Status = PharmacistStatus.Pending;
        }

        public void Approve(Guid pharmacyAdminId)
        {
            if (pharmacyAdminId == Guid.Empty)
                throw new ArgumentException("Pharmacy admin Id is required to approve a pharmacist.");
            if (ApprovedByAdminId.HasValue)
                throw new BusinessRuleViolationException("This pharmacist is already approved");

            if (string.IsNullOrWhiteSpace(SyndicateNumber) && string.IsNullOrWhiteSpace(SyndicateDocumentPath))
                throw new BusinessRuleViolationException("Can't approve a pharmacist without syndicate details.");

            ApprovedByAdminId = pharmacyAdminId;
            ApprovedAt = DateTime.UtcNow;
            Status = PharmacistStatus.Approved;
            MarkAsUpdated();
        }

        public void Reject()
        {
            if (Status != PharmacistStatus.Pending)
                throw new BusinessRuleViolationException("Can only reject pending pharmacists.");
            Status = PharmacistStatus.Rejected;
            MarkAsUpdated();
        }

        public void Reactivate()
        {
            if (Status != PharmacistStatus.Suspended)
                throw new BusinessRuleViolationException("Can only reactivate a suspended pharmacist.");
            Status = PharmacistStatus.Approved;
            MarkAsUpdated();
        }
        
        public void Deactivate()
        {
            if (Status != PharmacistStatus.Approved)
                throw new BusinessRuleViolationException("Can only suspend an approved pharmacist.");
            Status = PharmacistStatus.Suspended;
            MarkAsUpdated();
        }

        public void AssignToPharmacy(Guid pharmacyId)
        {
            if (pharmacyId == Guid.Empty)
                throw new ArgumentException("Pharmacy Id is required", nameof(pharmacyId));
            PharmacyId = pharmacyId;
            MarkAsUpdated();
        }


    }
}
