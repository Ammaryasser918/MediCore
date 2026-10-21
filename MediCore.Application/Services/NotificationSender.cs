using MediCore.Application.Common.Interfaces;
using MediCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Services
{
    public class NotificationSender : INotificationSender
    {
        readonly IEnumerable<INotificationChannelStrategy> _strategies;
        public NotificationSender(IEnumerable<INotificationChannelStrategy> strategies)
        {
            _strategies = strategies;
        }

        public async Task SendAsync(NotificationChannel channel, string recipient, string subject, string message, CancellationToken cancellationToken = default)
        {
            var strategy = _strategies.FirstOrDefault(s => s.Channel == channel) ?? throw new InvalidOperationException($"No notificaion strategy is registered for channel '{channel}'.");

            await strategy.SendAsync(recipient, subject, message, cancellationToken);
        }

    }
}
