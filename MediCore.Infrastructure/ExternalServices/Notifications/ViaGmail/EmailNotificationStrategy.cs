using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MediCore.Application.Common.Interfaces;
using MediCore.Domain.Enums;

namespace MediCore.Infrastructure.ExternalServices.Notifications.ViaGmail
{
    internal class EmailNotificationStrategy : INotificationChannelStrategy
    {
        readonly EmailOptions _options;
        public NotificationChannel Channel => NotificationChannel.Email;

        public EmailNotificationStrategy(IOptions<EmailOptions> options)
        {
            _options = options.Value;
        }


        public async Task SendAsync(string recipient, string subject, string message, CancellationToken cancellationToken = default)
        {
            var mail = new MimeMessage();
            mail.From.Add(new MailboxAddress(_options.SenderDisplayName, _options.SenderEmail));
            mail.To.Add(MailboxAddress.Parse(recipient));
            mail.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = message };
            mail.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            // الاتصال المباشر مع Gmail عبر STARTTLS
            await smtp.ConnectAsync(_options.SmtpHost, _options.SmtpPort, SecureSocketOptions.StartTls, cancellationToken);

            // المصادقة باستخدام كلمة سر التطبيقات (App Password)
            await smtp.AuthenticateAsync(_options.SenderEmail, _options.SenderPassword, cancellationToken);

            await smtp.SendAsync(mail, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);
        }


    }
}
