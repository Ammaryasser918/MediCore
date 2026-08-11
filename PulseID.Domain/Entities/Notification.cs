using PulseID.Domain.Common;
using PulseID.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; private set; } = default!;
        public NotificationType Type { get; private set; }
        public NotificationChannel Channel { get; private set; }
        public string Title { get; private set; } = default!;
        public string Message { get; private set; } = default!;
        public bool IsRead { get; private set; }
        public DateTime? ReadAt { get; private set; }
        public bool IsSent { get; private set; }
        public DateTime? SentAt { get; private set; }


        Notification() { }

        public Notification(Guid userId, NotificationType type, NotificationChannel channel, string title, string message)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User Id is required", nameof(userId));
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message is required", nameof(message));

            UserId = userId;
            Type = type;
            Channel = channel;
            Title = title;
            Message = message;

        }

        public void MarkAsSent()
        {
            if (IsSent) return;

            IsSent = true;
            SentAt = DateTime.UtcNow;
            MarkAsUpdated();
        }

        public void MarkAsRead()
        {
            if (IsRead) return; // to avoid update the ReadAt property again

            IsRead = true;
            ReadAt = DateTime.UtcNow;
            MarkAsUpdated();
        }


    }
}
