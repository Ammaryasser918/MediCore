using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.DTOs.Pharmacy
{
    public class CreatePharmacyDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = default!;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? LicenseNumber { get; set; }
        public string? LicenseDocumentPath { get; set; }
        

        public Guid? HospitalId {  get; set; }
    }
}
