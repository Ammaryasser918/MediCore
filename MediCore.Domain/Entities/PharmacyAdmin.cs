using MediCore.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Domain.Entities
{
    public class PharmacyAdmin : BaseEntity
    {
        public Guid UserId { get; private set; }
        public Guid PharmacyId { get; private set; }

        public Pharmacy Pharmacy { get; private set; } = null!;

        private PharmacyAdmin() { }

        public PharmacyAdmin(Guid userId, Guid pharmacyId)
        {
            UserId = userId;
            PharmacyId = pharmacyId;
        }
    }
}
