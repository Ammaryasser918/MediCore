using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PulseID.Application.DTOs.Hospital;
using PulseID.Application.DTOs.Department;
using PulseID.Application.Interfaces.IServices;

namespace PulseID.API.Controllers;


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
    public async Task<ActionResult<IReadOnlyList<HospitalDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var hospitals =
            await _hospitalService.GetAllAsync(cancellationToken);

        return Ok(hospitals);
    }


    // GET: api/hospital/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HospitalDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hospital =
            await _hospitalService.GetByIdAsync(
                id,
                cancellationToken);

        return Ok(hospital);
    }


    // POST: api/hospital
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HospitalDto>> Create(
        [FromBody] CreateHospitalDto dto,
        CancellationToken cancellationToken)
    {
        var created =
            await _hospitalService.CreateAsync(
                dto,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created);
    }


    // PUT: api/hospital/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HospitalDto>> Update(
        Guid id,
        [FromBody] UpdateHospitalDto dto,
        CancellationToken cancellationToken)
    {
        var updated =
            await _hospitalService.UpdateAsync(
                id,
                dto,
                cancellationToken);

        return Ok(updated);
    }


    // PUT: api/hospital/{id}/license
    [HttpPut("{id:guid}/license")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HospitalDto>> UpdateLicenseDocument(
        Guid id,
        [FromBody] string NewLicenseDocumentPath,
        CancellationToken cancellationToken)
    {
        var updated =
            await _hospitalService.UpdateLicenseDocumentAsync(
                id,
                NewLicenseDocumentPath,
                cancellationToken);

        return Ok(updated);
    }


    // DELETE: api/hospital/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _hospitalService.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }

    // PATCH: api/hospital/{id}/approve?adminId=...
    [HttpPatch("{id:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HospitalDto>> Approve(
        Guid id,
        [FromQuery] Guid adminId,
        CancellationToken cancellationToken)
    {
        var updated =
            await _hospitalService.ApproveAsync(
                id,
                adminId,
                cancellationToken);

        return Ok(updated);
    }


    // PATCH: api/hospital/{id}/reject
    [HttpPatch("{id:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HospitalDto>> Reject(
        Guid id,
        CancellationToken cancellationToken)
    {
        var updated =
            await _hospitalService.RejectAsync(
                id,
                cancellationToken);

        return Ok(updated);
    }


    // PATCH: api/hospital/{id}/deactivate
    [HttpPatch("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HospitalDto>> Deactivate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var updated =
            await _hospitalService.DeactivateAsync(
                id,
                cancellationToken);

        return Ok(updated);
    }


    // PATCH: api/hospital/{id}/reactivate
    [HttpPatch("{id:guid}/reactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HospitalDto>> Reactivate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var updated =
            await _hospitalService.ReactivateAsync(
                id,
                cancellationToken);

        return Ok(updated);
    }
}

