using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MediCore.Application.DTOs.Labs;
using MediCore.Application.Interfaces.Services;
using System.Security.Claims;

namespace MediCore.API.Controllers;

[ApiController]
[Route("api/labs")]
[Authorize]
public class LabController : ControllerBase
{
    readonly ILabService _labService;

    public LabController(ILabService labService)
    {
        _labService = labService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "SuperAdmin")]
    [EnableRateLimiting("BigDataLimiter")]
    public async Task<ActionResult<IReadOnlyList<LabDto>>> GetAll(CancellationToken cancellationToken)
    {
        var labs = await _labService.GetAllAsync(cancellationToken);
        return Ok(labs);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "SuperAdmin,LabTechnician,LabAdmin")]
    [EnableRateLimiting("SmallDataLimiter")]
    public async Task<ActionResult<LabDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var lab = await _labService.GetByIdAsync(id, cancellationToken);

        if (lab is null)
            return NotFound();

        return Ok(lab);
    }

    [HttpGet("{id:guid}/requests")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EnableRateLimiting("SmallDataLimiter")]
    [Authorize(Roles = "SuperAdmin,LabAdmin")]
    public async Task<ActionResult<LabDto>> GetWithRequests(Guid id, CancellationToken cancellationToken)
    {
        var lab = await _labService.GetWithRequestsAsync(id, cancellationToken);

        if (lab is null)
            return NotFound();

        return Ok(lab);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [AllowAnonymous]
    [EnableRateLimiting("AuthLimiter")]
    public async Task<ActionResult<LabDto>> Create(CreateLabDto dto, CancellationToken cancellationToken)
    {
        var lab = await _labService.CreateAsync(dto, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = lab.Id }, lab);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "SuperAdmin,LabAdmin")]
    [EnableRateLimiting("AuthLimiter")]
    public async Task<ActionResult<LabDto>> Update(Guid id, UpdateLabDto dto, CancellationToken cancellationToken)
    {
        var lab = await _labService.UpdateAsync(id, dto, cancellationToken);
        return Ok(lab);
    }

    [HttpPut("{id:guid}/license-document")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "SuperAdmin,LabAdmin")]
    [EnableRateLimiting("AuthLimiter")]
    public async Task<IActionResult> UpdateLicenseDocument(Guid id, [FromBody] string documentPath, CancellationToken cancellationToken)
    {
        await _labService.UpdateLicenseDocumentAsync(id, documentPath, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(adminIdClaim, out var labAdminId))
            return Unauthorized("Can't identify the current admin user.");

        await _labService.ApproveAsync(id, labAdminId, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Reject(Guid id, CancellationToken cancellationToken)
    {
        var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(adminIdClaim, out var labAdminId))
            return Unauthorized("Can't identify the current admin user.");

        await _labService.RejectAsync(id, labAdminId, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await _labService.DeactivateAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/reactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Reactivate(Guid id, CancellationToken cancellationToken)
    {
        await _labService.ReactivateAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _labService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}/hard-delete")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> HardDelete(Guid id, CancellationToken cancellationToken)
    {
        await _labService.HardDeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("deleted")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
    [EnableRateLimiting("BigDataLimiter")]
    public async Task<IActionResult> GetDeleted(CancellationToken cancellationToken)
    {
        var result = await _labService.GetDeletedLabsAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/restore")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
    [EnableRateLimiting("SmallDataLimiter")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        await _labService.RestoreLabAsync(id, cancellationToken);
        return Ok(new { Message = "Record restored successfully." });
    }

}