using MediCore.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Domain.Entities
{
    public class EmergencyContact : BaseEntity
    {
        public Guid PatientId { get; private set; }
        public Patient Patient { get; private set; } = default!;

        public string FullName { get; private set; } = default!;
        public string Relationship { get; private set; } = default!;

        public string PhoneNumber { get; private set; } = default!;

        public string? Address { get; private set; }

        EmergencyContact() { }

        public EmergencyContact(Guid patientId, string fullName, string relationship, string phoneNumber, string? address = null)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full Name is Required", nameof(fullName));
            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentException("Phone Number is Required", nameof(phoneNumber));

            PatientId = patientId;
            FullName = fullName;
            Relationship = relationship;
            PhoneNumber = phoneNumber;
            Address = address;

        }


        public void UpdateDetails(string fullName, string relation, string phoneNumber, string? address)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full Name is Required", nameof(fullName));
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                throw new ArgumentException("Phone Number is Required", nameof(phoneNumber));
            }
            FullName = fullName;
            Relationship = relation;
            PhoneNumber = phoneNumber;
            Address = address;
            MarkAsUpdated();
        }

    }
}
