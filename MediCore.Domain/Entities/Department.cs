using MediCore.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Domain.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; private set; } = default!;
        public string? Description { get; private set; }

        public Guid HospitalId { get; private set; }
        public Hospital Hospital { get; private set; } = default!;


        readonly List<Doctor> _doctors = new List<Doctor>();
        public IReadOnlyCollection<Doctor> Doctors => _doctors.AsReadOnly();

        Department() { }

        public Department(string name, Guid hospitalId, string? description = null)
        {
            Name = name;
            HospitalId = hospitalId;
            Description = description;
        }

        public void EditDepartment(string newName, string? description = null)
        {
            Name = newName;
            Description = description;
            MarkAsUpdated();
        }


    }
}
