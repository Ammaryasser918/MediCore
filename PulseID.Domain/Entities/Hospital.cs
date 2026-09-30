using PulseID.Domain.Common;
using PulseID.Domain.Enums;
using PulseID.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Domain.Entities
{
    public class Hospital : BaseEntity
    {
        public string Name { get; private set; } = default!;
        public string Address { get; private set; } = default!;
        public string PhoneNumber { get; private set; } = default!;
        public string? LicenseNumber { get; private set; } = default!;
        public string? LicenseDocumentPath { get; private set; }
        public HospitalStatus Status { get; private set; } = HospitalStatus.Pending;

        public Guid? ApprovedByAdminId { get; private set; }
        public DateTime? ApprovedAt { get; private set; }
        public Guid? RejectedByAdminId { get; private set; }
        public DateTime? RejectedAt { get; private set; }




        readonly List<Department> _departments = new List<Department>();
        public IReadOnlyCollection<Department> Departments => _departments.AsReadOnly();

        Hospital() { }

        public Hospital(string name, string address, string phoneNumber, string? licenseNumber = null, string? licenseDocumentPath = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Hospital Name is Required", nameof(name));
            }
            Name = name;
            Address = address;
            PhoneNumber = phoneNumber;
            LicenseNumber = licenseNumber;
            LicenseDocumentPath = licenseDocumentPath;
            Status = HospitalStatus.Pending;
        }

        public void UpdateDetails(string name, string address, string phoneNumber, string? licenseNumber = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Hospital Name is Required", nameof(name));

            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Hospital Address is Required", nameof(address));

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Hospital Phone Number is Required", nameof(phoneNumber));

            Name = name;
            Address = address;
            PhoneNumber = phoneNumber;
            LicenseNumber = licenseNumber;

            MarkAsUpdated();
        }


        public void UpdateLicenseDocument(string DocPath)
        {
            if (string.IsNullOrWhiteSpace(DocPath))
                throw new ArgumentException("License document path is required", nameof(DocPath));
            LicenseDocumentPath = DocPath;
            Status = HospitalStatus.Pending;
            ApprovedByAdminId = null;
            MarkAsUpdated();
        }

        public Department AddDepartment(string name, string? description = null)
        {
            if (_departments.Any(d => d.Name.ToLower() == name.ToLower()))
            {
                throw new InvalidOperationException("This Department Already Exists");
            }
            var Dept = new Department(name, Id, description);
            _departments.Add(Dept);
            MarkAsUpdated();
            return Dept;
        }

        public void RemoveDepartment(Guid DeptId)
        {
            var Dept = _departments.FirstOrDefault(d => d.Id == DeptId);
            if (Dept is not null)
            {
                _departments.Remove(Dept);
                MarkAsUpdated();
            }
        }

        public void Approve(Guid adminId)
        {
            if (adminId == Guid.Empty)
                throw new ArgumentException("Admin Id is required to approve a hospital.", nameof(adminId));
            if (ApprovedByAdminId.HasValue)
                throw new BusinessRuleViolationException("This hospital is already approved.");

            if (string.IsNullOrWhiteSpace(LicenseNumber) && string.IsNullOrWhiteSpace(LicenseDocumentPath))
                throw new BusinessRuleViolationException("Can't approve a hospital without license detials.");

            ApprovedByAdminId = adminId;
            ApprovedAt = DateTime.UtcNow;
            Status = HospitalStatus.Approved;
            MarkAsUpdated();

        }

        public void Reject(Guid adminId)
        {
            if (Status != HospitalStatus.Pending)
                throw new BusinessRuleViolationException("Can only reject pending hospitals");
            Status = HospitalStatus.Rejected;
            RejectedAt = DateTime.UtcNow;
            RejectedByAdminId = adminId;
            MarkAsUpdated();
        }


        public void Deactivate()
        {
            if (Status != HospitalStatus.Approved)
                throw new BusinessRuleViolationException("Can only suspend an approved hospital.");
            Status = HospitalStatus.Suspended;
            MarkAsUpdated();
        }

        public void Reactivate()
        {
            if (Status != HospitalStatus.Suspended)
                throw new BusinessRuleViolationException("Can only reactivate a suspended hospital.");
            Status = HospitalStatus.Approved;
            MarkAsUpdated();
        }


    }
}
