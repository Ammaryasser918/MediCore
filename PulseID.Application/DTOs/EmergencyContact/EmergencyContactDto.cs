using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.DTOs.EmergencyContact
{
    public class EmergencyContactDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string FullName { get; set; } = default!;
        public string Relationship { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string? Address { get; set; }
    }
}
