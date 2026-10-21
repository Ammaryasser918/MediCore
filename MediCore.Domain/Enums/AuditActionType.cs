using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Domain.Enums
{
    public enum AuditActionType
    {
        Create = 1,
        Update = 2,
        Delete = 3,
        View = 4,
        Login = 5,
        Logout = 6,
        AccessGranted = 7,
        AccessRevoked = 8,
        PrescriptionIssued = 9,
        PrescriptionDispensed = 10,
        FileDownloaded = 11
    }
}
