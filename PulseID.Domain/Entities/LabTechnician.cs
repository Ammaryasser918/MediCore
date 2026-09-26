using PulseID.Domain.Common;
using PulseID.Domain.Enums;
using PulseID.Domain.Exceptions;

namespace PulseID.Domain.Entities
{
    public class LabTechnician : BaseEntity
    {
        public Guid UserId { get; private set; }

        public string FullName { get; private set; } = default!;
        public string NationalId { get; private set; } = default!;

        public string LicenseNumber { get; private set; } = default!;
        public string? LicenseDocumentPath { get; private set; }

        public string PhoneNumber { get; private set; } = default!;
        public string? PersonalImagePath { get; private set; }

        public LabTechnicianStatus Status { get; private set; } = LabTechnicianStatus.Pending;

        public Guid LabId { get; private set; }
        public Lab Lab { get; private set; } = default!;

        public Guid? ApprovedByHospitalAdminId { get; private set; }
        public DateTime? ApprovedAt { get; private set; }

        LabTechnician() { }

        public LabTechnician(
            Guid userId,
            string fullName,
            string nationalId,
            string licenseNumber,
            string? licenseDocumentPath,
            string phoneNumber,
            Guid labId,
            string? personalImagePath = null)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User Id is Required", nameof(userId));

            if (labId == Guid.Empty)
                throw new ArgumentException("Lab Id is Required", nameof(labId));

            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full Name is Required", nameof(fullName));

            if (string.IsNullOrWhiteSpace(nationalId))
                throw new ArgumentException("National Id is Required", nameof(nationalId));

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone Number is Required", nameof(phoneNumber));

            UserId = userId;
            FullName = fullName;
            NationalId = nationalId;
            LicenseNumber = licenseNumber;
            LicenseDocumentPath = licenseDocumentPath;
            PhoneNumber = phoneNumber;
            LabId = labId;
            PersonalImagePath = personalImagePath;

            Status = LabTechnicianStatus.Pending;
        }

        public void UpdatePersonalDetails(string phoneNumber, string? personalImagePath)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone Number is Required", nameof(phoneNumber));

            PhoneNumber = phoneNumber;

            if (!string.IsNullOrWhiteSpace(personalImagePath))
                PersonalImagePath = personalImagePath;

            MarkAsUpdated();
        }

        public void Approve(Guid labHospitalId)
        {
            if (labHospitalId == Guid.Empty)
                throw new ArgumentException("Lab Hospital Id is Required", nameof(labHospitalId));

            if (ApprovedByHospitalAdminId.HasValue)
                throw new BusinessRuleViolationException("This technician is already approved.");

            if (string.IsNullOrWhiteSpace(LicenseNumber) &&
                string.IsNullOrWhiteSpace(LicenseDocumentPath))
                throw new BusinessRuleViolationException("Can't approve a technician without license details.");

            ApprovedByHospitalAdminId = labHospitalId;
            ApprovedAt = DateTime.UtcNow;
            Status = LabTechnicianStatus.Approved;

            MarkAsUpdated();
        }

        public void Reject()
        {
            if (Status != LabTechnicianStatus.Pending)
                throw new BusinessRuleViolationException("Can only reject pending technicians.");

            Status = LabTechnicianStatus.Rejected;

            MarkAsUpdated();
        }

        public void Deactivate()
        {
            if (Status != LabTechnicianStatus.Approved)
                throw new BusinessRuleViolationException("Can only suspend an approved technician.");

            Status = LabTechnicianStatus.Suspended;

            MarkAsUpdated();
        }

        public void Reactivate()
        {
            if (Status != LabTechnicianStatus.Suspended)
                throw new BusinessRuleViolationException("Can only reactivate a suspended technician.");

            Status = LabTechnicianStatus.Approved;

            MarkAsUpdated();
        }

        public void TransferToLab(Guid labId)
        {
            if (labId == Guid.Empty)
                throw new ArgumentException("Lab Id is Required", nameof(labId));

            LabId = labId;

            MarkAsUpdated();
        }
    }
}