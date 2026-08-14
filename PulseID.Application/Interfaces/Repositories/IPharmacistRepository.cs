using PulseID.Application.Common.Interfaces;
using PulseID.Domain.Entities;
using PulseID.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.Repositories
{
    public interface IPharmacistRepository : IRepository<Pharmacist>
    {
        Task<Pharmacist?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Pharmacist?> GetBySyndicateNumberAsync(string syndicateNumber, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Pharmacist>> GetByStatusAsync(PharmacistStatus status, CancellationToken cancellationToken = default);
    }
}
