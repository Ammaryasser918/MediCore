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
using PulseID.Application.Common.Interfaces;
using PulseID.Domain.Common;

namespace PulseID.Application.Services
{
    public class AccessValidationService : IAccessValidationService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly ICurrentUserService _currentUserService;
        public AccessValidationService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> HasAccessAsync(Patient patient, Guid requesterId, AccessScope requiredScope, CancellationToken cancellationToken = default)
        {

            if (patient.UserId == requesterId)
                return true;

            if (_currentUserService.Role == AppRoles.SuperAdmin || _currentUserService.Role == AppRoles.HospitalAdmin) return true;

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
