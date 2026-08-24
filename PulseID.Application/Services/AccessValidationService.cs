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

namespace PulseID.Application.Services
{
    public class AccessValidationService : IAccessValidationService
    {
        readonly IUnitOfWork _unitOfWork;
        public AccessValidationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> HasAccessAsync(Guid patientId, Guid requesterId, AccessScope requiredScope, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(patientId, cancellationToken);
            if (patient is null) return false;

            if (patient.UserId == requesterId)
                return true;

            var authorization = await _unitOfWork.AccessAuthorizations.GetActiveAuthorizationAsync(patientId, requesterId, cancellationToken);

            if (authorization is null || !authorization.IsValidNow()) return false;

            return (authorization.Scope == AccessScope.FullMedicalRecord) || (authorization.Scope == requiredScope);
        }

        public async Task EnsureAccessAsync(Guid patientId, Guid requesterId, AccessScope requiredScope, CancellationToken cancellationToken = default)
        {
            if (!await HasAccessAsync(patientId, requesterId, requiredScope, cancellationToken))
                throw new ForbiddenAccessException("You don't have an approved authorization to this patient's medical record");
        }

    }
}
