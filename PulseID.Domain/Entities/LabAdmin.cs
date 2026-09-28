using PulseID.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Domain.Entities
{
    public class LabAdmin : BaseEntity
    {
        public Guid UserId { get; private set; }
        public Guid LabId { get; private set; }
        public Lab Lab { get; private set; } = null!;

        private LabAdmin() { }

        public LabAdmin(Guid userId, Guid labId)
        {
            UserId = userId;
            LabId = labId;
        }
    }
}
