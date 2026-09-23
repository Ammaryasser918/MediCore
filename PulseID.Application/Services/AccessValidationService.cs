using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Enums;
using PulseID.Application.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using PulseID.Domain.Entities;

namespace PulseID.Application.Services
{
    public class AccessValidationService : IAccessValidationService
    {
        readonly IUnitOfWork _unitOfWork;
        public AccessValidationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> HasAccessAsync(Patient patient, Guid requesterId, AccessScope requiredScope, CancellationToken cancellationToken = default)
        {

            if (patient.UserId == requesterId)
                return true;

            var authorization = await _unitOfWork.AccessAuthorizations.GetActiveAuthorizationAsync(patient.Id, requesterId, cancellationToken);

            if (authorization is null || !authorization.IsValidNow()) return false;

            return (authorization.Scope == AccessScope.FullMedicalRecord) || (authorization.Scope == requiredScope);
        }

        public async Task EnsureAccessAsync(Patient patient, Guid requesterId, AccessScope requiredScope, CancellationToken cancellationToken = default)
        {
            if (!await HasAccessAsync(patient, requesterId, requiredScope, cancellationToken))
                throw new ForbiddenAccessException("You don't have an approved authorization to this patient's medical record");
        }

    }
}
