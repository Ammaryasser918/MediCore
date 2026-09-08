using PulseID.Domain.Common;
using PulseID.Domain.Enums;
using PulseID.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Domain.Entities
{
    public class AccessAuthorization : BaseEntity
    {
        public Guid PatientId { get; private set; }
        public Patient Patient { get; private set; } = default!;
        public Guid GranteeUserId { get; private set; } = default!;
        public string GranteeRole { get; private set; } = default!;
        public string Purpose { get; private set; }
        public int RequestedDurationMinutes { get; private set; }

        public AccessScope Scope { get; private set; }
        public AuthorizationStatus Status { get; private set; } = AuthorizationStatus.Pending;

        public DateTime RequestedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? RespondedAt { get; private set; }
        public DateTime? ExpiresAt { get; private set; } 
        public string? RejectionReason { get; private set; }


        AccessAuthorization() { }

        public AccessAuthorization(Guid patientId, Guid granteeUserId, string granteeRole, AccessScope scope,
            string purpose, int requestedDurationMinutes)
        {
            if (patientId == Guid.Empty)
                throw new ArgumentException("Patient Id is required", nameof(patientId));
            if (granteeUserId == Guid.Empty)
                throw new ArgumentException("Grantee User Id is required", nameof(granteeUserId));
            if (string.IsNullOrWhiteSpace(granteeRole))
                throw new ArgumentException("Grantee Role is required", nameof(granteeRole));
            if (string.IsNullOrWhiteSpace(purpose))
                throw new ArgumentException("Purpuse is required", nameof(purpose));
            if (requestedDurationMinutes <= 0)
                throw new ArgumentException("Requested duration must be greater than zero", nameof(requestedDurationMinutes));

            PatientId = patientId;
            GranteeUserId = granteeUserId;
            GranteeRole = granteeRole;
            Scope = scope;
            Purpose = purpose;
            RequestedDurationMinutes = requestedDurationMinutes;
        }

        public void Approve()
        {
            if (Status != AuthorizationStatus.Pending)
                throw new BusinessRuleViolationException("Can only approve access in pending case");
            Status = AuthorizationStatus.Approved;
            RespondedAt = DateTime.UtcNow;
            ExpiresAt = RespondedAt.Value.AddMinutes(RequestedDurationMinutes);
            MarkAsUpdated();
        }

        public void Reject(string reason)
        {
            if (Status != AuthorizationStatus.Pending)
                throw new BusinessRuleViolationException("Can only reject access in pending case");
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Rejection reason is required", nameof(reason));

            Status = AuthorizationStatus.Rejected;
            RejectionReason = reason;
            RespondedAt = DateTime.UtcNow;
            MarkAsUpdated();
            

        }

        public void Revoke()
        {
            if (Status != AuthorizationStatus.Approved)
                throw new BusinessRuleViolationException("Can't Revoke Access Request at pending case");
            Status = AuthorizationStatus.Revoked;
            MarkAsUpdated();
        }
        
        public bool IsValidNow() =>
        Status == AuthorizationStatus.Approved &&
        (!ExpiresAt.HasValue || ExpiresAt > DateTime.UtcNow);
    }


}
