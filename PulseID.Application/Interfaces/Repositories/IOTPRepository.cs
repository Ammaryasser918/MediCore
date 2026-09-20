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
    public interface IOTPRepository : IRepository<OTP>
    {
        Task<OTP?> GetLatestActiveAsync(string email, OtpPurpose purpose, CancellationToken cancellationToken = default);
    }
}
