using MediCore.Domain.Common;
using MediCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; } = default!;
        public string UserRole { get; private set; } = default!;
        public AuditActionType ActionType { get; private set; }
        public string EntityName { get; private set; } = default!;
        public Guid? EntityId { get; private set; }
        public string? Details { get; private set; }
        public string? IPAddress { get; private set; }
        public DateTime Timestamp { get; private set; } = DateTime.UtcNow;


        AuditLog() { }

        public AuditLog(Guid userId, string userRole, AuditActionType actionType, string entityName,
                         Guid? entityId = null, string? details = null, string? ipAddress = null)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User Id is Required", nameof(userId));
            if (string.IsNullOrWhiteSpace(userRole))
                throw new ArgumentException("User Role is required", nameof(userRole));
            if (string.IsNullOrWhiteSpace(entityName))
                throw new ArgumentException("Entity Name is required", nameof(entityName));

            UserId = userId;
            UserRole = userRole;
            ActionType = actionType;
            EntityName = entityName;
            EntityId = entityId;
            Details = details;
            IPAddress = ipAddress;
            Timestamp = DateTime.UtcNow;
        }


    }
}
