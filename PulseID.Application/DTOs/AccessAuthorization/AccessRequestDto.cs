using PulseID.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.DTOs.AccessAuthorization
{
    public class AccessRequestDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid GranteeUserId { get; set; }
        public string Purpose { get; set; } = default!;
        public AccessScope Scope { get; set; }
        public AuthorizationStatus Status { get; set; }
        public int RequestedDurationMinutes { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
