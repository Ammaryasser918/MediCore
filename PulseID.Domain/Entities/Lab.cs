using PulseID.Domain.Common;
using PulseID.Domain.Enums;
using PulseID.Domain.Exceptions;

namespace PulseID.Domain.Entities
{
    public class Lab : BaseEntity
    {
        public string Name { get; private set; } = default!;
        public string Address { get; private set; } = default!;
        public string PhoneNumber { get; private set; } = default!;

        public string? LicenseNumber { get; private set; }
        public string? LicenseDocumentPath { get; private set; }

        public LabStatus Status { get; private set; } = LabStatus.Pending;

        public Guid? ApprovedByHospitalAdminId { get; private set; }
        public DateTime? ApprovedAt { get; private set; }

        readonly List<LabTechnician> _labTechnicians = new();
        public IReadOnlyCollection<LabTechnician> LabTechnicians => _labTechnicians.AsReadOnly();

        readonly List<LabTestRequest> _testRequests = new();
        public IReadOnlyCollection<LabTestRequest> TestRequests => _testRequests.AsReadOnly();


        Lab() { }

        public Lab(
            string name,
            string address,
            string phoneNumber,
            string? licenseNumber = null,
            string? licenseDocumentPath = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Lab Name is Required", nameof(name));

            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Lab Address is Required", nameof(address));

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Lab Phone Number is Required", nameof(phoneNumber));

            Name = name;
            Address = address;
            PhoneNumber = phoneNumber;
            LicenseNumber = licenseNumber;
            LicenseDocumentPath = licenseDocumentPath;
            Status = LabStatus.Pending;
        }

        public void UpdateDetails(
            string name,
            string address,
            string phoneNumber,
            string? licenseNumber = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Lab Name is Required", nameof(name));

            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Lab Address is Required", nameof(address));

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Lab Phone Number is Required", nameof(phoneNumber));

            Name = name;
            Address = address;
            PhoneNumber = phoneNumber;
            LicenseNumber = licenseNumber;

            MarkAsUpdated();
        }

        public void UpdateLicenseDocument(string documentPath)
        {
            if (string.IsNullOrWhiteSpace(documentPath))
                throw new ArgumentException("License document path is required", nameof(documentPath));

            LicenseDocumentPath = documentPath;

            Status = LabStatus.Pending;
            ApprovedByHospitalAdminId = null;
            ApprovedAt = null;

            MarkAsUpdated();
        }

        public void Approve(Guid adminId)
        {
            if (adminId == Guid.Empty)
                throw new ArgumentException("Admin Id is required to approve a lab.", nameof(adminId));

            if (ApprovedByHospitalAdminId.HasValue)
                throw new BusinessRuleViolationException("This lab is already approved.");

            if (string.IsNullOrWhiteSpace(LicenseNumber) &&
                string.IsNullOrWhiteSpace(LicenseDocumentPath))
            {
                throw new BusinessRuleViolationException(
                    "Can't approve a lab without license details.");
            }

            ApprovedByHospitalAdminId = adminId;
            ApprovedAt = DateTime.UtcNow;
            Status = LabStatus.Approved;

            MarkAsUpdated();
        }

        public void Reject()
        {
            if (Status != LabStatus.Pending)
                throw new BusinessRuleViolationException(
                    "Can only reject pending labs.");

            Status = LabStatus.Rejected;

            MarkAsUpdated();
        }

        public void Deactivate()
        {
            if (Status != LabStatus.Approved)
                throw new BusinessRuleViolationException(
                    "Can only suspend an approved lab.");

            Status = LabStatus.Suspended;

            MarkAsUpdated();
        }

        public void Reactivate()
        {
            if (Status != LabStatus.Suspended)
                throw new BusinessRuleViolationException(
                    "Can only reactivate a suspended lab.");

            Status = LabStatus.Approved;

            MarkAsUpdated();
        }
    }
}