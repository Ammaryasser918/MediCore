using PulseID.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Domain.Entities
{
    public class HospitalAdmin : BaseEntity
    {
        public Guid UserId { get; private set; }
        public Guid HospitalId { get; private set; }

        public Hospital Hospital { get; private set; } = null!;

        private HospitalAdmin() { } // For EF Core

        public HospitalAdmin(Guid userId, Guid hospitalId)
        {
            UserId = userId;
            HospitalId = hospitalId;
        }
    }
}
