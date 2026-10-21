using MediCore.Application.Common.Interfaces;
using MediCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Interfaces.Repositories
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<Patient?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Patient?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default);

        Task<bool> ExistsByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default);

        // Gets the patient with the full profile (Allergies, Chronic Diseases, Emergency Contacts, ...)
        Task<Patient?> GetWithFullProfileAsync(Guid patientId, CancellationToken cancellationToken = default);

    }
}
