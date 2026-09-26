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
        public DateTime DateOfBirth { get; private set; }
        public int Age {  get; private set; }
        public PharmacistStatus Status { get; private set; }
        public string PhoneNumber { get; private set; } = default!;
        public DateTime? ShiftStartedAt { get; private set; }
        public DateTime? ShiftEndedAt { get; private set; }
        public Guid? PharmacyId { get; private set; }
        public Pharmacy Pharmacy { get; private set; } = default!;
        public Guid? ApprovedByAdminId { get; private set; }
        public DateTime? ApprovedAt { get; private set; }
        public Guid? RejectedByAdminId { get; private set; }
        public DateTime? RejectedAt { get; private set; }


        readonly List<PrescriptionItem> _dispensedItems = new List<PrescriptionItem>();
        public IReadOnlyCollection<PrescriptionItem> PrescriptionItems => _dispensedItems.AsReadOnly();

        Pharmacist() { }

        public Pharmacist(Guid userId, string fullName, string syndicateNumber, string syndicateDocumentPath, 
            string nationalId, DateTime dateOfBirth, string phoneNumber, Guid? pharmacyId = null)
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
            DateOfBirth = dateOfBirth;
            Status = PharmacistStatus.Pending;
        }


        public void UpdatePersonalDetails(string phoneNumber, DateTime dateOfBirth)
        {
            PhoneNumber = phoneNumber;
            DateOfBirth = dateOfBirth;
            MarkAsUpdated();
        }

        public void Approve(Guid adminId)
        {
            if (adminId == Guid.Empty)
                throw new ArgumentException("Pharmacy admin Id is required to approve a pharmacist.");
            if (ApprovedByAdminId.HasValue)
                throw new BusinessRuleViolationException("This pharmacist is already approved");

            if (string.IsNullOrWhiteSpace(SyndicateNumber) && string.IsNullOrWhiteSpace(SyndicateDocumentPath))
                throw new BusinessRuleViolationException("Can't approve a pharmacist without syndicate details.");

            ApprovedByAdminId = adminId;
            ApprovedAt = DateTime.UtcNow;
            Status = PharmacistStatus.Approved;
            MarkAsUpdated();
        }

        public void Reject(Guid adminId)
        {
            if (adminId == Guid.Empty)
                throw new ArgumentException("Pharmacy admin Id is required to approve a pharmacist.");

            if (Status != PharmacistStatus.Pending)
                throw new BusinessRuleViolationException("Can only reject pending pharmacists.");
            Status = PharmacistStatus.Rejected;
            MarkAsUpdated();
        }

        public void Reactivate(Guid adminId)
        {
            if (adminId == Guid.Empty)
                throw new ArgumentException("Pharmacy admin Id is required to approve a pharmacist.");

            if (Status != PharmacistStatus.Suspended)
                throw new BusinessRuleViolationException("Can only reactivate a suspended pharmacist.");
            Status = PharmacistStatus.Approved;
            MarkAsUpdated();
        }
        
        public void Deactivate(Guid adminId)
        {
            if (adminId == Guid.Empty)
                throw new ArgumentException("Pharmacy admin Id is required to approve a pharmacist.");

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

        public int GetAge()
        {
            return DateTime.UtcNow.Year - DateOfBirth.Year - (DateTime.UtcNow.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
        }


        public void StartShift()
        {
            ShiftStartedAt = DateTime.UtcNow;
            ShiftEndedAt = null;
            MarkAsUpdated();
        }

        public void EndShift()
        {
            ShiftEndedAt = DateTime.UtcNow;
            MarkAsUpdated();
        }

        public bool IsShiftActive => ShiftStartedAt.HasValue &&
            (!ShiftEndedAt.HasValue || ShiftStartedAt.Value > ShiftEndedAt.Value);


    }
}
