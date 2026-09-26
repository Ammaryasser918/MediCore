using PulseID.Domain.Common;
using PulseID.Domain.Enums;
using PulseID.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace PulseID.Domain.Entities
{
    public class Pharmacy : BaseEntity
    {
        public string Name { get; private set; } = default!;
        public string Email { get; private set; } = default!;
        public string Address { get; private set; } = default!;
        public string PhoneNumber { get; private set; } = default!;
        public string? LicenseNumber { get; private set; } = default!;
        public string? LicenseDocumentPath { get; private set; }
        public PharmacyStatus Status { get; private set; } = PharmacyStatus.Pending;
        public Guid? HospitalId { get; private set; } 
        public Hospital? Hospital { get; private set; } 

        public Guid? ApprovedByAdminId { get; private set; }
        public DateTime? ApprovedAt { get; private set; }
        public Guid? RejectedByAdminId { get; private set; } 
        public DateTime? RejectedAt { get; private set; }




        readonly List<Pharmacist> _pharmacists = new List<Pharmacist>();
        public IReadOnlyCollection<Pharmacist> Pharmacists => _pharmacists.AsReadOnly();



        Pharmacy() { }

        public Pharmacy(string name, string email, string address, string phoneNumber, string? licenseNumber = null, string? licenseDocumentPath = null, Guid? hospitalId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Pharmacy name is required.", nameof(name));
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number is required.", nameof(phoneNumber));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            Name = name;
            Email = email;
            Address = address;
            PhoneNumber = phoneNumber;
            LicenseNumber = licenseNumber;
            LicenseDocumentPath = licenseDocumentPath;
            HospitalId = hospitalId;
            Status = PharmacyStatus.Pending;
        }


        public void UpdateLicenseDocument(string DocPath)
        {
            if (string.IsNullOrWhiteSpace(DocPath))
                throw new ArgumentException("License document is required.", nameof(DocPath));
            LicenseDocumentPath = DocPath;
            Status = PharmacyStatus.Pending;
            ApprovedByAdminId = null;
            MarkAsUpdated();
        }

        public void Approve(Guid adminId)
        {
            if (adminId == Guid.Empty)
                throw new ArgumentException("Admin Id is required to approve a pharmacy.", nameof(adminId));
            if (ApprovedByAdminId.HasValue)
                throw new BusinessRuleViolationException("This pharmacy is already approved.");

            if (string.IsNullOrWhiteSpace(LicenseNumber) && string.IsNullOrWhiteSpace(LicenseDocumentPath))
                throw new BusinessRuleViolationException("Can't approve a pharmacy without license details.");
            
            ApprovedByAdminId = adminId;
            ApprovedAt = DateTime.UtcNow;
            Status = PharmacyStatus.Approved;
            MarkAsUpdated();
        }

        public void Reject(Guid adminId)
        {
            if (adminId == Guid.Empty)
                throw new ArgumentException("Admin Id is required to reject a pharmacy.", nameof(adminId));

            if (Status != PharmacyStatus.Pending)
                throw new BusinessRuleViolationException("Can only reject pending pharmacies.");
            Status = PharmacyStatus.Rejected;
            MarkAsUpdated();
        }

        public void Deactivate(Guid adminId)
        {
            if (adminId == Guid.Empty)
                throw new ArgumentException("Admin Id is required to deactivate a pharmacy.", nameof(adminId));

            if (Status != PharmacyStatus.Approved)
                throw new BusinessRuleViolationException("Can only suspend an approved pharmacy");
            Status = PharmacyStatus.Suspended;
            MarkAsUpdated();
        }

        public void Reactivate(Guid adminId)
        {
            if (adminId == Guid.Empty)
                throw new ArgumentException("Admin Id is required to reactivate a pharmacy.", nameof(adminId));


            if (Status != PharmacyStatus.Suspended)
                throw new BusinessRuleViolationException("Can only reactivate a suspended pharmacy.");
            Status = PharmacyStatus.Approved;
            MarkAsUpdated();
        }

        public void AssignToHospital(Guid hospitalId)
        {
            if (hospitalId == Guid.Empty)
                throw new ArgumentException("Hospital Id is required.", nameof(hospitalId));
            HospitalId = hospitalId;
            MarkAsUpdated();
        }



    }
}
