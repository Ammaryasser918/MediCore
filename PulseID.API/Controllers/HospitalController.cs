using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PulseID.Application.DTOs.Department;
using PulseID.Application.DTOs.Hospital;
using PulseID.Application.Interfaces.IServices;
using PulseID.Application.Services;
using System;
using System.Security.Claims;

namespace PulseID.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class HospitalController : ControllerBase
{
    readonly IHospitalService _hospitalService;

    public HospitalController(IHospitalService hospitalService)
    {
        _hospitalService = hospitalService;
    }


    // GET: api/hospital
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,PharmacyAdmin,LabAdmin,Doctor,Patient,Pharmacist,LabTechnician")]
    public async Task<ActionResult<IReadOnlyList<HospitalDto>>> GetAll(CancellationToken cancellationToken)
    {
        var hospitals =
            await _hospitalService.GetAllAsync(cancellationToken);

        return Ok(hospitals);
    }


    // GET: api/hospital/{id}
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,PharmacyAdmin,LabAdmin,Doctor,Patient,Pharmacist,LabTechnician")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HospitalDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var hospital =
            await _hospitalService.GetByIdAsync(id, cancellationToken);

        return Ok(hospital);
    }


    // POST: api/hospital
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<ActionResult<HospitalDto>> Create([FromBody] CreateHospitalDto dto, CancellationToken cancellationToken)
    {
        var created =
            await _hospitalService.CreateAsync(dto, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }


    // PUT: api/hospital/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "HospitalAdmin,SuperAdmin")]
    public async Task<ActionResult<HospitalDto>> Update(Guid id, [FromBody] UpdateHospitalDto dto, CancellationToken cancellationToken)
    {
        var updated =
            await _hospitalService.UpdateAsync(id, dto, cancellationToken);

        return Ok(updated);
    }


    // PUT: api/hospital/{id}/license
    [HttpPut("{id:guid}/license")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "HospitalAdmin,SuperAdmin")]
    public async Task<ActionResult<HospitalDto>> UpdateLicenseDocument(Guid id, [FromBody] string NewLicenseDocumentPath, CancellationToken cancellationToken)
    {
        var updated =
            await _hospitalService.UpdateLicenseDocumentAsync(id, NewLicenseDocumentPath, cancellationToken);

        return Ok(updated);
    }


    // DELETE: api/hospital/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _hospitalService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }

    // PATCH: api/hospital/{id}/approve?adminId=...
    [HttpPatch("{id:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<HospitalDto>> Approve(Guid id, CancellationToken cancellationToken)
    {
        var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(adminIdClaim, out var adminId))
            return Unauthorized("Can't identify the current admin user.");

        var updated =
            await _hospitalService.ApproveAsync(id, adminId, cancellationToken);
        return Ok(updated);
    }

    [HttpGet("{id:guid}/departments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,PharmacyAdmin,LabAdmin,Doctor,Patient,Pharmacist,LabTechnician")]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetDepartments(Guid id, CancellationToken cancellationToken)
    {
        var departments =
            await _hospitalService.GetDepartmentsAsync(id, cancellationToken);

        return Ok(departments);
    }

    [HttpGet("{id:guid}/departments/details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetDepartmentsWithDetails(Guid id, CancellationToken cancellationToken)
    {
        var departments =
            await _hospitalService.GetDepartmentsWithDetailsAsync(id, cancellationToken);

        return Ok(departments);
    }


    // PATCH: api/hospital/{id}/reject
    [HttpPatch("{id:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<HospitalDto>> Reject(Guid id, CancellationToken cancellationToken)
    {

        var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(adminIdClaim, out var adminId))
            return Unauthorized("Can't identify the current admin user.");

        var updated =
            await _hospitalService.RejectAsync(id, adminId, cancellationToken);

        return Ok(updated);
    }


    // PATCH: api/hospital/{id}/deactivate
    [HttpPatch("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<HospitalDto>> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var updated =
            await _hospitalService.DeactivateAsync(id, cancellationToken);

        return Ok(updated);
    }


    // PATCH: api/hospital/{id}/reactivate
    [HttpPatch("{id:guid}/reactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<HospitalDto>> Reactivate(Guid id, CancellationToken cancellationToken)
    {
        var updated =
            await _hospitalService.ReactivateAsync(id, cancellationToken);
        
        return Ok(updated);
    }

    [HttpDelete("hard-delete")]
    [Authorize("SuperAdmin")]
    public async Task<IActionResult> HardDelete(Guid id, CancellationToken cancellationToken)
    {
        await _hospitalService.HardDeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("deleted")]
    [Authorize(Roles = "SuperAdmin")]
    [EnableRateLimiting("BigDataLimiter")]
    public async Task<IActionResult> GetDeleted(CancellationToken cancellationToken)
    {
        var result = await _hospitalService.GetDeletedAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/restore")]
    [Authorize(Roles = "SuperAdmin")]
    [EnableRateLimiting("SmallDataLimiter")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        await _hospitalService.RestoreHospitalAsync(id, cancellationToken);
        return Ok(new { Message = "Record restored successfully." });
    }

}

