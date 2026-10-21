using MediCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.DTOs.Pharmacist
{
    public class PharmacistDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string SyndicateNumber { get; set; } = default!;
        public string SyndicateDocumentPath { get; set; } = default!;
        public string NationalId { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public PharmacistStatus Status { get; set; }
        public string PhoneNumber { get; set; } = default!;
        public Guid? PharmacyId { get; set; } 
        public string? PharmacyName { get; set; }
        public Guid? ApprovedByAdminId { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? RejectedByAdminId { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string? PersonalImageUrl { get; set; }


    }
}
