using PulseID.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Domain.Entities
{
    public class Allergy : BaseEntity
    {
        public string Name { get; private set; } = default!;
        public string? Description { get; private set; }

        Allergy() { }

        public Allergy(string name, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Allergy Name is Required", nameof(name));
            }
            Name = name;
            Description = description;
        }


    }
}
