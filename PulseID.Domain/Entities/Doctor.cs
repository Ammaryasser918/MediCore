using PulseID.Domain.Common;
using PulseID.Domain.Enums;
using PulseID.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Domain.Entities
{
    public class Doctor : BaseEntity
    {

        public Guid UserId { get; private set; } = default!;

        public string FullName { get; private set; } = default!;
        public string SyndicateNumber { get; private set; } = default!;
        public string? SyndicateDocumentPath { get; private set; }
        public string NationalId { get; private set; } = default!;
        public string Specialization { get; private set; } = default!;
        public string PhoneNumber { get; private set; } = default!;
        public string? PersonalImagePath { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public int Age { get; private set; }
        public DoctorStatus Status { get; private set; } = DoctorStatus.Pending;

        public Guid? ApprovedByHospitalAdminId { get; private set; }
        public DateTime? ApprovedAt { get; private set; }

        public Guid? RejectedByAdminId { get; private set; }
        public DateTime? RejectedAt { get; private set;  }
        readonly List<LabTestRequest> _labTestRequests = new List<LabTestRequest>();
        public IReadOnlyCollection<LabTestRequest> LabTestRequests => _labTestRequests.AsReadOnly();

        public Guid? DepartmentId { get; private set; }
        public Department? Department { get; private set; } = default!;


        Doctor() { }

        public Doctor(Guid userId, string fullName, string syndicateNumber, string syndicateDocumentPath, string nationalId, 
            string specialization, string phoneNumber, DateTime dateOfBirth, Guid? departmentId = null, string? personalImagePath = null)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User Id is Required", nameof(userId));
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full Name is required", nameof(fullName));
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number is required", nameof(phoneNumber));
            if (string.IsNullOrWhiteSpace(nationalId))
                throw new ArgumentException("National Id is required", nameof(nationalId));

            UserId = userId;
            FullName = fullName;
            SyndicateNumber = syndicateNumber;
            SyndicateDocumentPath = syndicateDocumentPath;
            Specialization = specialization;
            PhoneNumber = phoneNumber;
            DepartmentId = departmentId;
            PersonalImagePath = personalImagePath;
            NationalId = nationalId;
            DateOfBirth = dateOfBirth;
            Status = DoctorStatus.Pending;
        }

        public void UpdatePersonalDetails(string fullName, string nationalId, string phoneNumber, string specialization, DateTime dateOfBirth, string? personalImagePath)
        {
            FullName = fullName;
            NationalId = nationalId;
            PhoneNumber = phoneNumber;
            Specialization = specialization;
            DateOfBirth = dateOfBirth;
            if (!string.IsNullOrWhiteSpace(personalImagePath))
                PersonalImagePath = personalImagePath;
            MarkAsUpdated();
        }

        public void Approve(Guid adminId)
        {
            if (adminId == Guid.Empty)
                throw new ArgumentException("Hospital admin Id is required to approve a doctor.");
            if (ApprovedByHospitalAdminId.HasValue)
                throw new BusinessRuleViolationException("This doctor is already approved");

            if (string.IsNullOrWhiteSpace(SyndicateNumber) && string.IsNullOrWhiteSpace(SyndicateDocumentPath))
                throw new BusinessRuleViolationException("Can't approve a doctor without syndicate details.");

            ApprovedByHospitalAdminId = adminId;
            ApprovedAt = DateTime.UtcNow;
            Status = DoctorStatus.Approved;
            MarkAsUpdated();
        }

        public void Deactivate(Guid adminId)
        {
            if (adminId == Guid.Empty)
                throw new ArgumentException("Hospital admin Id is required to approve a doctor.");

                if (Status != DoctorStatus.Approved)
                throw new BusinessRuleViolationException("Can only suspend an approved doctor.");
            Status = DoctorStatus.Suspended;
            MarkAsUpdated();
        }

        public void Reactivate(Guid adminId)
        {
            if (adminId == Guid.Empty)
                throw new ArgumentException("Hospital admin Id is required to approve a doctor.");

                if (Status != DoctorStatus.Suspended)
                throw new BusinessRuleViolationException("Can only reactivate a suspended Doctor.");
            Status = DoctorStatus.Approved;
            MarkAsUpdated();
        }

        public void Reject(Guid adminId)
        {
            if (adminId == Guid.Empty)
                throw new ArgumentException("Hospital admin Id is required to approve a doctor.");

                if (Status != DoctorStatus.Pending)
                throw new BusinessRuleViolationException("Can only reject pending doctors.");
            Status = DoctorStatus.Rejected;
            MarkAsUpdated();
        }

        public void TransferToDepartment(Guid departmentId)
        {
            if (departmentId == Guid.Empty)
                throw new ArgumentException("Department Id is Required", nameof(departmentId));

            DepartmentId = departmentId;
            MarkAsUpdated();
        }


        public int GetAge()
        {
            return DateTime.UtcNow.Year - DateOfBirth.Year - (DateTime.UtcNow.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
        }


    }
}
