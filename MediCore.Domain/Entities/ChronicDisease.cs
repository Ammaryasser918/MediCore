using MediCore.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Domain.Entities
{
    public class ChronicDisease : BaseEntity
    {
        public string Name { get; private set; } = default!;
        public string? Description { get; private set; }


        ChronicDisease() { }

        public ChronicDisease(string name, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Chronic Disease Name is Required.", nameof(name));
            Name = name;
            Description = description;
        }

    }
}
