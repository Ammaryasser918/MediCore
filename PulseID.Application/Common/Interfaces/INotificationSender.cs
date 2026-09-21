using PulseID.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Common.Interfaces
{
    public interface INotificationSender
    {
        Task SendAsync(NotificationChannel channel, string recipient, string subject, string message, CancellationToken cancellationToken = default);
    }
}
