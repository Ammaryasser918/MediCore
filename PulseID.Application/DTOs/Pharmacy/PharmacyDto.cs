using PulseID.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.DTOs.Pharmacy
{
    public class PharmacyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? LicenseNumber { get; set; }
        public string? LicenseDocumentPath { get; set; }
        public PharmacyStatus PharmacyStatus { get; set; }

        public Guid? HospitalId { get; set; }
        public string? HospitalName { get; set; }


        public Guid? ApprovedByAdminId { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


    }
}
