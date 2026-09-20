using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PulseID.Application.DTOs.Doctor;
using PulseID.Application.DTOs.Pharmacist;
using PulseID.Application.Interfaces.IServices;
using PulseID.Application.Services;
using System.Security.Claims;

namespace PulseID.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacistController : ControllerBase
    {
        readonly IPharmacistService _pharmacistService;
        readonly IAuthorizationService _authorizationService;

        public PharmacistController(IPharmacistService pharmacistService, IAuthorizationService authorizationService)
        {
            _pharmacistService = pharmacistService;
            _authorizationService = authorizationService;
        }



        // GET: api/pharmacists
        [Authorize(Roles = "SuperAdmin,PharmacyAdmin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [EnableRateLimiting("BigDataLimiter")]
        public async Task<ActionResult<IReadOnlyList<PharmacistDto>>> GetAll(CancellationToken cancellationToken)
        {
            var pharmacists = await _pharmacistService.GetAllAsync(cancellationToken);
            return Ok(pharmacists);
        }


        // GET: api/pharmacists/{id}
        [Authorize(Roles = "Pharmacist,SuperAdmin,PharmacyAdmin")]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EnableRateLimiting("SmallDataLimiter")]
        public async Task<ActionResult<PharmacistDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var pharmacist = await _pharmacistService.GetByIdAsync(id, cancellationToken);
            if (pharmacist is null) return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, pharmacist, "PharmacistOwnerOrAdmin");
            if (!authResult.Succeeded)
                return Forbid();

            return Ok(pharmacist);
        }

        //POST: api/pharmacists
        [Authorize(Roles = "PharmacyAdmin,SuperAdmin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PharmacistDto>> Create([FromBody] CreatePharmacistDto dto, CancellationToken cancellationToken)
        {
            var created = await _pharmacistService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/pharmacists/{id}/pharmacy
        [Authorize(Roles = "SuperAdmin,PharmacyAdmin")]
        [HttpPut("{id:guid}/pharmacy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PharmacistDto>> AssignToPharmacy(Guid id, [FromBody] AssignPharmacistToPharmacyDto dto, CancellationToken cancellationToken)
        {
            var updated = await _pharmacistService.AssignToPharmacyAsync(id, dto, cancellationToken);
            return Ok(updated);
        }

        // PUT: api/pharmacists/{id}
        [Authorize(Roles = "Pharmacist,PharmacyAdmin,SuperAdmin")]
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PharmacistDto>> Update(Guid id, [FromBody] UpdatePharmacistDto dto, CancellationToken cancellationToken)
        {
            var existingPharmacist = await _pharmacistService.GetByIdAsync(id, cancellationToken);
            if (existingPharmacist is null) return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, existingPharmacist, "PharmacistOwnerOrAdmin");
            if (!authResult.Succeeded) return Forbid();

            var updated = await _pharmacistService.UpdatePharmacistDetailsAsync(id, dto, cancellationToken);
            return Ok(updated);
        }


        // DELETE: api/Pharmacists/{id}
        [Authorize(Roles = "PharmacyAdmin,SuperAdmin")]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _pharmacistService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        // PATCH: api/pharmacists/{id}/approve?pharmacyAdminId=...
        [Authorize(Roles = "PharmacyAdmin,SuperAdmin")]
        [HttpPatch("{id:guid}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PharmacistDto>> Approve(Guid id, CancellationToken cancellationToken)
        {
            var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(adminIdClaim, out var pharmacyAdminId))
                return Unauthorized("Can't identify the current admin user.");

            var updated = await _pharmacistService.ApproveAsync(id, pharmacyAdminId, cancellationToken);
            return Ok(updated);
        }


        // PATCH: api/pharmacists/{id}/reject
        [Authorize(Roles = "PharmacyAdmin,SuperAdmin")]
        [HttpPatch("{id:guid}/reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PharmacistDto>> Reject(Guid id, CancellationToken cancellationToken)
        {
            var updated = await _pharmacistService.RejectAsync(id, cancellationToken);
            return Ok(updated);
        }

        // PATCH: api/pharmacists/{id}/deactivate
        [Authorize(Roles = "PharmacyAdmin,SuperAdmin")]
        [HttpPatch("{id:guid}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PharmacistDto>> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var updated = await _pharmacistService.DeactivateAsync(id, cancellationToken);
            return Ok(updated);
        }

        // PATCH: api/pharmacists/{id}/reactivate
        [Authorize(Roles = "PharmacyAdmin,SuperAdmin")]
        [HttpPatch("{id:guid}/reactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PharmacistDto>> Reactivate(Guid id, CancellationToken cancellationToken)
        {
            var updated = await _pharmacistService.ReactivateAsync(id, cancellationToken);
            return Ok(updated);
        }




    }
}
