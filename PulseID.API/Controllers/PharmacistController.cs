using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PulseID.Application.DTOs.Doctor;
using PulseID.Application.DTOs.Patient;
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


        /// <summary>
        /// Retrieves the profile of the currently authenticated pharmacist.
        /// </summary>
        // GET: api/pharmacist/me
        [Authorize(Roles = "Pharmacist")]
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EnableRateLimiting("SmallDataLimiter")]
        public async Task<ActionResult<PharmacistDto>> GetCurrentPharmacist(CancellationToken cancellationToken)
        {
            var Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(Id, out var userId))
                return Unauthorized("Can't identify the current user.");

            var pharmacist = await _pharmacistService.GetByUserIdAsync(userId, cancellationToken);
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
        [Authorize(Roles = "PharmacyAdmin,SuperAdmin,Pharmacist")]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var existingPharmacist = await _pharmacistService.GetByIdAsync(id, cancellationToken);
            if (existingPharmacist is null) return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, existingPharmacist, "PharmacistOwnerOrAdmin");
            if (!authResult.Succeeded) return Forbid();
            await _pharmacistService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:guid}/hard-delete")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> HardDelete(Guid id, CancellationToken cancellationToken)
        {
            await _pharmacistService.HardDeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpGet("deleted")]
        [Authorize(Roles = "SuperAdmin,PharmacyAdmin")]
        [EnableRateLimiting("BigDataLimiter")]
        public async Task<IActionResult> GetDeleted(CancellationToken cancellationToken)
        {
            var result = await _pharmacistService.GetDeletedPharmacistsAsync(cancellationToken);
            return Ok(result);
        }

        [HttpPost("{id:guid}/restore")]
        [Authorize(Roles = "SuperAdmin,PharmacyAdmin")]
        [EnableRateLimiting("SmallDataLimiter")]
        public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken)
        {
            await _pharmacistService.RestorePharmacistAsync(id, cancellationToken);
            return Ok(new { Message = "Record restored successfully." });
        }


        [Authorize(Roles = "Pharmacist,SuperAdmin,PharmacyAdmin")]
        [HttpPut("{id:guid}/personal-image")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PharmacistDto>> UploadPersonalImage(Guid id, IFormFile file, CancellationToken cancellationToken)
        {
            var existing = await _pharmacistService.GetByIdAsync(id, cancellationToken);
            if (existing is null) return NotFound();
            var auth = await _authorizationService.AuthorizeAsync(User, existing, "PharmacistOwnerOrAdmin");
            if (!auth.Succeeded) return Forbid();
            return Ok(await _pharmacistService.SetPersonalImageAsync(id, file, cancellationToken));
        }

        [Authorize(Roles = "Pharmacist,SuperAdmin,PharmacyAdmin")]
        [HttpPut("{id:guid}/personal-image/remove")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PharmacistDto>> RemovePersonalImage(Guid id, IFormFile file, CancellationToken cancellationToken)
        {
            var existing = await _pharmacistService.GetByIdAsync(id, cancellationToken);
            if (existing is null) return NotFound();
            var auth = await _authorizationService.AuthorizeAsync(User, existing, "PharmacistOwnerOrAdmin");
            if (!auth.Succeeded) return Forbid();
            return Ok(await _pharmacistService.RemovePersonalImageAsync(id, cancellationToken));
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
            var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(adminIdClaim, out var pharmacyAdminId))
                return Unauthorized("Can't identify the current admin user.");

            var updated = await _pharmacistService.RejectAsync(id, pharmacyAdminId, cancellationToken);
            return Ok(updated);
        }

        // PATCH: api/pharmacists/{id}/deactivate
        [Authorize(Roles = "PharmacyAdmin,SuperAdmin")]
        [HttpPatch("{id:guid}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PharmacistDto>> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(adminIdClaim, out var pharmacyAdminId))
                return Unauthorized("Can't identify the current admin user.");

            var updated = await _pharmacistService.DeactivateAsync(id, pharmacyAdminId, cancellationToken);
            return Ok(updated);
        }

        // PATCH: api/pharmacists/{id}/reactivate
        [Authorize(Roles = "PharmacyAdmin,SuperAdmin")]
        [HttpPatch("{id:guid}/reactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PharmacistDto>> Reactivate(Guid id, CancellationToken cancellationToken)
        {
            var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(adminIdClaim, out var pharmacyAdminId))
                return Unauthorized("Can't identify the current admin user.");

            var updated = await _pharmacistService.ReactivateAsync(id, pharmacyAdminId, cancellationToken);
            return Ok(updated);
        }

        [HttpPost("start-shift")]
        [Authorize(Roles = "Pharmacist,PharmacyAdmin,SuperAdmin")]
        public async Task<IActionResult> StartShift([FromQuery] Guid? pharmacistId, CancellationToken cancellationToken)
        {
            await _pharmacistService.StartShiftAsync(pharmacistId, cancellationToken);
            return Ok(new { Message = "Shift started successfully." });
        }

        [HttpPost("end-shift")]
        [Authorize(Roles = "Pharmacist,PharmacyAdmin,SuperAdmin")]
        public async Task<IActionResult> EndShift([FromQuery] Guid? pharmacistId, CancellationToken cancellationToken)
        {
            await _pharmacistService.EndShiftAsync(pharmacistId, cancellationToken);
            return Ok(new { Message = "Shift ended successfully." });
        }


        [HttpGet("shift-info")]
        [Authorize]
        [ProducesResponseType(typeof(PharmacistShiftInfoDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetShiftInfo([FromQuery] Guid? pharmacistId, CancellationToken cancellationToken)
        {
            var result = await _pharmacistService.GetShiftInfoAsync(pharmacistId, cancellationToken);
            return Ok(result);
        }

    }
}
