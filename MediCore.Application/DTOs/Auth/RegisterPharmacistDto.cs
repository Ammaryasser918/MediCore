using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.DTOs.Auth
{
    public class RegisterPharmacistDto
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;

        public string FullName { get; set; } = default!;
        public string NationalId { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } = default!;
        public string SyndicateNumber { get; set; } = default!;
        public string SyndicateDocumentPath { get; set; } = default!;
        
        public Guid? PharmacyId { get; set; }

    }
}
