using PulseID.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.DTOs.Pharmacist
{
    public class PharmacistDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; } = default!;
        public string SyndicateNumber { get; set; } = default!;
        public string SyndicateDocumentPath { get; set; } = default!;
        public string NationalId { get; set; } = default!;
        public PharmacistStatus Status { get; set; }
        public string PhoneNumber { get; set; } = default!;
        public Guid? PharmacyId { get; set; } 
        public string? PharmacyName { get; set; }
        public Guid? ApprovedByAdminId { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
    }
}
