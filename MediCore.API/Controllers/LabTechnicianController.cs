using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MediCore.Application.DTOs.Doctor;
using MediCore.Application.DTOs.Labs;
using MediCore.Application.Interfaces.Services;
using System.Security.Claims;

namespace MediCore.API.Controllers;

[Authorize]
[ApiController]
[Route("api/lab-technicians")]
public class LabTechnicianController : ControllerBase
{
    readonly ILabTechnicianService _labTechnicianService;
    readonly IAuthorizationService _authorizationService;

    public LabTechnicianController(ILabTechnicianService labTechnicianService, IAuthorizationService authorizationService)
    {
        _labTechnicianService = labTechnicianService;
        _authorizationService = authorizationService;
    }

    [HttpGet]
    [Authorize(Roles = "LabAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [EnableRateLimiting("BigDataLimiter")]
    public async Task<ActionResult<IReadOnlyList<LabTechnicianDto>>> GetAll(CancellationToken cancellationToken)
    {
        var technicians = await _labTechnicianService.GetAllAsync(cancellationToken);
        return Ok(technicians);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "LabAdmin,LabTechnician,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [EnableRateLimiting("SmallDataLimiter")]
    public async Task<ActionResult<LabTechnicianDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var technician = await _labTechnicianService.GetByIdAsync(id, cancellationToken);

        if (technician is null)
            return NotFound();
        var auth = await _authorizationService.AuthorizeAsync(User, technician, "LabTechnicianOwnerOrAdmin");
        if (!auth.Succeeded)
            return Forbid();

        return Ok(technician);
    }

    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EnableRateLimiting("SmallDataLimiter")]
    [Authorize(Roles = "LabTechnician")]
    public async Task<ActionResult<LabTechnicianDto>> GetCurrentTechnician(CancellationToken cancellationToken)
    {
        var Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(Id, out var userId))
            return Unauthorized("Can't identify the current user.");
        var technician = await _labTechnicianService.GetByUserIdAsync(userId, cancellationToken);

        if (technician is null)
            return NotFound();

        return Ok(technician);
    }

    [HttpGet("by-lab/{labId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [EnableRateLimiting("BigDataLimiter")]
    [Authorize(Roles = "LabAdmin,SuperAdmin")]
    public async Task<ActionResult<IReadOnlyList<LabTechnicianDto>>> GetByLab(Guid labId, CancellationToken cancellationToken)
    {
        var technicians = await _labTechnicianService.GetByLabAsync(labId, cancellationToken);
        return Ok(technicians);
    }

    [HttpPost]
    [Authorize(Roles = "LabAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LabTechnicianDto>> Create(CreateLabTechnicianDto dto, CancellationToken cancellationToken)
    {
        var technician = await _labTechnicianService.CreateAsync(dto, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = technician.Id }, technician);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "LabAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, UpdateLabTechnicianDto dto, CancellationToken cancellationToken)
    {
        var existingTechnician = await _labTechnicianService.GetByIdAsync(id, cancellationToken);
        if (existingTechnician is null) return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(User, existingTechnician, "LabTechnicianOwnerOrAdmin");
        if (!auth.Succeeded)
            return Forbid();
        await _labTechnicianService.UpdateAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/transfer/{labId:guid}")]
    [Authorize(Roles = "LabAdmin,SuperAdmin,LabTechnician")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TransferToLab(Guid id, Guid labId, CancellationToken cancellationToken)
    {
        var existingTechnician = await _labTechnicianService.GetByIdAsync(id, cancellationToken);
        if (existingTechnician is null) return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(User, existingTechnician, "LabTechnicianOwnerOrAdmin");
        if (!auth.Succeeded)
            return Forbid();
        await _labTechnicianService.TransferToLabAsync(id, labId, cancellationToken);
        return NoContent();
    }


    [Authorize(Roles = "LabTechnician,SuperAdmin,LabAdmin")]
    [HttpPut("{id:guid}/personal-image")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<LabTechnicianDto>> UploadPersonalImage(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        var existing = await _labTechnicianService.GetByIdAsync(id, cancellationToken);
        if (existing is null) return NotFound();
        var auth = await _authorizationService.AuthorizeAsync(User, existing, "LabTechnicianOwnerOrAdmin");
        if (!auth.Succeeded) return Forbid();
        return Ok(await _labTechnicianService.SetPersonalImageAsync(id, file, cancellationToken));
    }

    [Authorize(Roles = "LabTechnician,SuperAdmin,LabAdmin")]
    [HttpPut("{id:guid}/personal-image/remove")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<LabTechnicianDto>> RemovePersonalImage(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        var existing = await _labTechnicianService.GetByIdAsync(id, cancellationToken);
        if (existing is null) return NotFound();
        var auth = await _authorizationService.AuthorizeAsync(User, existing, "LabTechnicianOwnerOrAdmin");
        if (!auth.Succeeded) return Forbid();
        return Ok(await _labTechnicianService.RemovePersonalImageAsync(id, cancellationToken));
    }


    [HttpPatch("{id:guid}/approve")]
    [Authorize(Roles = "LabAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(adminIdClaim, out var labAdminId))
            return Unauthorized("Can't identify the current admin user.");

        await _labTechnicianService.ApproveAsync(id, labAdminId, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/reject")]
    [Authorize(Roles = "LabAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(Guid id, CancellationToken cancellationToken)
    {
        var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(adminIdClaim, out var labAdminId))
            return Unauthorized("Can't identify the current admin user.");

        await _labTechnicianService.RejectAsync(id, labAdminId, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/deactivate")]
    [Authorize(Roles = "LabAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await _labTechnicianService.DeactivateAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/reactivate")]
    [Authorize(Roles = "LabAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reactivate(Guid id, CancellationToken cancellationToken)
    {
        await _labTechnicianService.ReactivateAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "LabAdmin,SuperAdmin,LabTechnician")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var existingTechnician = await _labTechnicianService.GetByIdAsync(id, cancellationToken);
        if (existingTechnician is null) return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(User, existingTechnician, "LabTechnicianOwnerOrAdmin");
        if (!auth.Succeeded)
            return Forbid();
        await _labTechnicianService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}/hard-delete")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> HardDelete(Guid id, CancellationToken cancellationToken)
    {
        await _labTechnicianService.HardDeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("deleted")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
    [EnableRateLimiting("BigDataLimiter")]
    public async Task<IActionResult> GetDeleted(CancellationToken cancellationToken)
    {
        var result = await _labTechnicianService.GetDeletedTechniciansAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/restore")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
    [EnableRateLimiting("SmallDataLimiter")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        await _labTechnicianService.RestoreTechnicianAsync(id, cancellationToken);
        return Ok(new { Message = "Record restored successfully." });
    }
}