using MediCore.Application.Common.Interfaces;
using MediCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Interfaces.Repositories
{
    public interface IEmergencyContactRepository : IRepository<EmergencyContact>
    {
        Task<IEnumerable<EmergencyContact>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    }
}
