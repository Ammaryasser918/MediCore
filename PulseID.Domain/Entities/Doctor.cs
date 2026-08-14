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
        public string Specialization { get; private set; } = default!;
        public string PhoneNumber { get; private set; } = default!;
        public string? PersonalImagePath { get; private set; }
        public DoctorStatus Status { get; private set; } = DoctorStatus.Pending;

        public Guid? ApprovedByHospitalAdminId { get; private set; } 
        public DateTime? ApprovedAt { get; private set; }
        public Guid DepartmentId { get; private set; }
        public Department Department { get; private set; } = default!;


        Doctor() { }

        public Doctor(Guid userId, string fullName, string syndicateNumber, string syndicateDocumentPath, string specialization,
            string phoneNumber, Guid departmentId, string? personalImagePath = null)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User Id is Required", nameof(userId));
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full Name is required", nameof(fullName));
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number is required", nameof(phoneNumber));

            UserId = userId;
            FullName = fullName;
            SyndicateNumber = syndicateNumber;
            SyndicateDocumentPath = syndicateDocumentPath;
            Specialization = specialization;
            PhoneNumber = phoneNumber;
            DepartmentId = departmentId;
            PersonalImagePath = personalImagePath;
            Status = DoctorStatus.Pending;
        }

        public void Approve(Guid hospitalAdminId)
        {
            if (hospitalAdminId == Guid.Empty)
                throw new ArgumentException("Hospital admin Id is required to approve a doctor.");
            if (ApprovedByHospitalAdminId.HasValue)
                throw new BusinessRuleViolationException("This doctor is already approved");

            if (string.IsNullOrWhiteSpace(SyndicateNumber) && string.IsNullOrWhiteSpace(SyndicateDocumentPath))
                throw new BusinessRuleViolationException("Can't approve a doctor without syndicate details.");

            ApprovedByHospitalAdminId = hospitalAdminId;
            ApprovedAt = DateTime.UtcNow;
            Status = DoctorStatus.Approved;
            MarkAsUpdated();
        }

        public void Deactivate()
        {
            if (Status != DoctorStatus.Approved)
                throw new BusinessRuleViolationException("Can only suspend an approved doctor.");
            Status = DoctorStatus.Suspended;
            MarkAsUpdated();
        }

        public void Reactivate()
        {
            if (Status != DoctorStatus.Suspended)
                throw new BusinessRuleViolationException("Can only reactivate a suspended Doctor.");
            Status = DoctorStatus.Approved;
            MarkAsUpdated();
        }

        public void Reject()
        {
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
        




    }
}
