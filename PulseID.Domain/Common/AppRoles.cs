using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Domain.Common
{
    public static class AppRoles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string HospitalAdmin = "HospitalAdmin";
        public const string PharmacyAdmin = "PharmacyAdmin";
        public const string Doctor = "Doctor";
        public const string Patient = "Patient";
        public const string Pharmacist = "Pharmacist";
    }
}
