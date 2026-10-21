using MediCore.Application.Common.Interfaces;
using MediCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Interfaces.Repositories
{
    public interface IAllergyRepository : IRepository<Allergy>
    {
        Task<Allergy?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Allergy>> SearchByNameAsync(string term, CancellationToken cancellationToken = default);
    }
}
