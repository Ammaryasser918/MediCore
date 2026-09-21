using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Infrastructure.ExternalServices.Notifications.ViaGmail
{
    public class EmailOptions
    {
        public string SmtpHost { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public string SenderEmail { get; set; } = default!;

        public string SenderPassword { get; set; } = default!;
        public string SenderDisplayName { get; set; } = default!;
        public bool EnableSsl { get; set; } = true;

    }
}
