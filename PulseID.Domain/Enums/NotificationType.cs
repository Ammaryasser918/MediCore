using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Domain.Enums
{
    public enum NotificationType
    {
        OtpCode = 1,
        PrescriptionReady = 2,
        AccessRequest = 3,
        AccessApproved = 4,
        AccessRejected = 5,
        AppointmentReminder = 6,
        SystemAlert = 7,
        RecordUpdated = 8
    }
}
