using MediCore.Application.Common.Interfaces;
using MediCore.Domain.Entities;
using MediCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Interfaces.Repositories
{
    public interface IPharmacyRepository : IRepository<Pharmacy>
    {
        Task<IReadOnlyList<Pharmacy>> GetByStatusAsync(PharmacyStatus status, CancellationToken cancellationToken = default);
    }
}
