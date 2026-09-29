using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.DTOs.Pharmacist
{
    public class UpdatePharmacistDto
    {
        public string FullName { get; set; } = default!;
        public string NationalId { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
    }
}
