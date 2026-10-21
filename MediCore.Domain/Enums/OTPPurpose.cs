using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Domain.Enums
{
    public enum OtpPurpose
    {
        Login = 1,
        Registration = 2,
        PasswordReset = 3,
        AccessAuthorization = 4,
        TransactionVerification = 5
    }
}
