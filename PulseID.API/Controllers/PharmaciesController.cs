using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PulseID.Application.DTOs.Pharmacy;
using PulseID.Application.Interfaces.IServices;
using System.Security.Claims;

namespace PulseID.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PharmaciesController : ControllerBase
    {
        readonly IPharmacyService _pharmacyService;
        public PharmaciesController(IPharmacyService pharmacyService)
        {
            _pharmacyService = pharmacyService;
        }


        // GET: api/pharmacies
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "SuperAdmin")]
        [EnableRateLimiting("BigDataLimiter")]
        public async Task<ActionResult<IReadOnlyList<PharmacyDto>>> GetAll(CancellationToken cancellationToken)
        {
            var pharmacies = await _pharmacyService.GetAllAsync(cancellationToken);
            return Ok(pharmacies);
        }


        // GET: api/pharmacies/{id}
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "SuperAdmin,PharmacyAdmin,Pharmacist")]
        [EnableRateLimiting("SmallDataLimiter")]
        public async Task<ActionResult<PharmacyDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var pharmacy = await _pharmacyService.GetByIdAsync(id, cancellationToken);
            return Ok(pharmacy);
        }



        // POST: api/pharmacies
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<PharmacyDto>> Create([FromBody] CreatePharmacyDto dto, CancellationToken cancellationToken)
        {
            var created = await _pharmacyService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }



        // DELETE: api/pharmacies/{id}/approve?adminId=...
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _pharmacyService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }



        // PATCH: api/pharmacies/{id}/approve?adminId=...
        [HttpPatch("{id:guid}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<PharmacyDto>> Approve(Guid id, CancellationToken cancellationToken)
        {
            var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(adminIdClaim, out var adminId))
                return Unauthorized("Can't identify the current admin user.");

            var updated = await _pharmacyService.ApproveAsync(id, adminId, cancellationToken);
            return Ok(updated);
        }



        // PATCH: api/pharmacies/{id}/reject
        [HttpPatch("{id:guid}/reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<PharmacyDto>> Reject(Guid id, CancellationToken cancellationToken)
        {
            var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(adminIdClaim, out var adminId))
                return Unauthorized("Can't identify the current admin user.");

            var updated = await _pharmacyService.RejectAsync(id, adminId, cancellationToken);
            return Ok(updated);
        }



        // PATCH: api/pharmacies/{id}/deactivate
        [HttpPatch("{id:guid}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<PharmacyDto>> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(adminIdClaim, out var adminId))
                return Unauthorized("Can't identify the current admin user.");

            var updated = await _pharmacyService.DeactivateAsync(id, adminId, cancellationToken);
            return Ok(updated);
        }



        // PATCH: api/pharmacies/{id}/reactivate
        [HttpPatch("{id:guid}/reactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<PharmacyDto>> Reactivate(Guid id, CancellationToken cancellationToken)
        {
            var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(adminIdClaim, out var adminId))
                return Unauthorized("Can't identify the current admin user.");

            var updated = await _pharmacyService.ReactivateAsync(id, adminId, cancellationToken);
            return Ok(updated);
        }
        


    }
}
