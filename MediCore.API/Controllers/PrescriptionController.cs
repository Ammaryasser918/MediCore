using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MediCore.Application.DTOs.Prescriptions;
using MediCore.Application.Interfaces.Services;
using MediCore.Domain.Entities;
using MediCore.Domain.Enums;
using System.Security.Claims;

namespace MediCore.API.Controllers;

[ApiController]
[Route("api/prescriptions")]
[Authorize]
public class PrescriptionController : ControllerBase
{
    readonly IPrescriptionService _prescriptionService;
    readonly IAuthorizationService _authorizationService;

    public PrescriptionController(IPrescriptionService prescriptionService, IAuthorizationService authorizationService)
    {
        _prescriptionService = prescriptionService;
        _authorizationService = authorizationService;
    }

    [HttpGet("by-medical-record/{medicalRecordId:guid}")]
    [Authorize(Roles = "Patient,Doctor,Pharmacist,PharmacyAdmin,HospitalAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [EnableRateLimiting("BigDataLimiter")]
    public async Task<ActionResult<IReadOnlyList<PrescriptionDto>>> GetByMedicalRecord(Guid medicalRecordId, CancellationToken cancellationToken)
    {
        var prescriptions = await _prescriptionService.GetByMedicalRecordAsync(medicalRecordId, cancellationToken);
        var authorized = new List<PrescriptionDto>();
        foreach (var prescription in prescriptions)
        {
            var auth = await _authorizationService.AuthorizeAsync(User, prescription, "PrescriptionOwnerOrAdmin");
            if (auth.Succeeded)
            {
                authorized.Add(prescription);
            }
        }
        return Ok(authorized);
    }

    [HttpGet("by-status/{status}")]
    [Authorize(Roles = "Pharmacist,PharmacyAdmin,HospitalAdmin,SuperAdmin,Patient")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [EnableRateLimiting("BigDataLimiter")]
    public async Task<ActionResult<IReadOnlyList<PrescriptionDto>>> GetByStatus(PrescriptionStatus status, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(typeof(PrescriptionStatus), status))
        {
            return BadRequest("Invalid prescription status.");
        }

        var prescriptions = await _prescriptionService.GetByStatusAsync(status, cancellationToken);
        var authorized = new List<PrescriptionDto>();
        foreach (var prescription in prescriptions)
        {
            var auth = await _authorizationService.AuthorizeAsync(User, prescription, "PrescriptionOwnerOrAdmin");
            if (auth.Succeeded)
            {
                authorized.Add(prescription);
            }
        }
        return Ok(authorized);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Patient,Doctor,Pharmacist,PharmacyAdmin,HospitalAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [EnableRateLimiting("SmallDataLimiter")]
    public async Task<ActionResult<PrescriptionDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var prescription = await _prescriptionService.GetByIdAsync(id, cancellationToken);

        if (prescription is null)
            return NotFound();
        var auth = await _authorizationService.AuthorizeAsync(User, prescription, "PrescriptionOwnerOrAdmin");
        if (!auth.Succeeded)
            return Forbid();

        return Ok(prescription);
    }

    [HttpGet("{id:guid}/items")]
    [Authorize(Roles = "Patient,Doctor,Pharmacist,PharmacyAdmin,HospitalAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [EnableRateLimiting("SmallDataLimiter")]
    public async Task<ActionResult<PrescriptionDto>> GetWithItems(Guid id, CancellationToken cancellationToken)
    {
        var prescription = await _prescriptionService.GetWithItemsAsync(id, cancellationToken);

        if (prescription is null)
            return NotFound();
        var auth = await _authorizationService.AuthorizeAsync(User, prescription, "PrescriptionOwnerOrAdmin");
        if (!auth.Succeeded)
            return Forbid();

        return Ok(prescription);
    }

    [HttpPost]
    [Authorize(Roles = "Doctor,HospitalAdmin,SuperAdmin,Pharmacist,PharmacyAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PrescriptionDto>> Create(CreatePrescriptionDto dto, CancellationToken cancellationToken)
    {
        var prescription = await _prescriptionService.CreateAsync(dto, cancellationToken);
        var auth = await _authorizationService.AuthorizeAsync(User, prescription, "PrescriptionOwnerOrAdmin");
        if (!auth.Succeeded)
            return Forbid();
        return CreatedAtAction(nameof(GetById), new { id = prescription.Id }, prescription);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Doctor,HospitalAdmin,SuperAdmin,Pharmacist,PharmacyAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PrescriptionDto>> Update(Guid id, UpdatePrescriptionDto dto, CancellationToken cancellationToken)
    {
        var prescription = await _prescriptionService.UpdateAsync(id, dto, cancellationToken);
        if (prescription is null)
            return NotFound();
        var auth = await _authorizationService.AuthorizeAsync(User, prescription, "PrescriptionOwnerOrAdmin");
        if (!auth.Succeeded)
            return Forbid();
        return Ok(prescription);
    }

    [HttpPost("{id:guid}/items")]
    [Authorize(Roles = "Doctor,HospitalAdmin,SuperAdmin,Pharmacist,PharmacyAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddItem(Guid id, AddPrescriptionItemDto dto, CancellationToken cancellationToken)
    {
        var existingPrescription = await _prescriptionService.GetByIdAsync(id, cancellationToken);
        if (existingPrescription is null)
            return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(User, existingPrescription, "PrescriptionOwnerOrAdmin");
        if (!auth.Succeeded)
            return Forbid();
        await _prescriptionService.AddItemAsync(id, dto, cancellationToken);
        return Ok(new { Message = "Prescription item added successfully.", PrescriptionId = id });
    }

    [HttpPut("{id:guid}/items/{itemId:guid}")]
    [Authorize(Roles = "Doctor,HospitalAdmin,SuperAdmin,Pharmacist,PharmacyAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItem(Guid id, Guid itemId, UpdatePrescriptionItemDto dto, CancellationToken cancellationToken)
    {
        var existingPrescription = await _prescriptionService.GetByIdAsync(id, cancellationToken);
        if (existingPrescription is null)
            return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(User, existingPrescription, "PrescriptionOwnerOrAdmin");
        if (!auth.Succeeded)
            return Forbid();
        await _prescriptionService.UpdateItemAsync(id, itemId, dto, cancellationToken);
        return Ok(new { Message = "Prescription item updated successfully.", PrescriptionId = id, ItemId = itemId });
    }

    [HttpDelete("{id:guid}/items/{itemId:guid}")]
    [Authorize(Roles = "Doctor,HospitalAdmin,SuperAdmin,Pharmacist,PharmacyAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveItem(Guid id, Guid itemId, CancellationToken cancellationToken)
    {
        var existingPrescription = await _prescriptionService.GetByIdAsync(id, cancellationToken);
        if (existingPrescription is null)
            return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(User, existingPrescription, "PrescriptionOwnerOrAdmin");
        if (!auth.Succeeded)
            return Forbid();

        await _prescriptionService.RemoveItemAsync(id, itemId, cancellationToken);
        return Ok(new { Message = "Prescription item removed successfully.", PrescriptionId = id, ItemId = itemId });
    }

    [HttpPut("{id:guid}/verify")]
    [Authorize(Roles = "Doctor,HospitalAdmin,SuperAdmin,Pharmacist,PharmacyAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Verify(Guid id, CancellationToken cancellationToken)
    {
        var existing = await _prescriptionService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(User, existing, "PrescriptionOwnerOrAdmin");
        if (!auth.Succeeded)
            return Forbid();

        await _prescriptionService.VerifyAsync(id, cancellationToken);
        return Ok(new { Message = "Prescription verified successfully.", PrescriptionId = id });
    }

    [HttpPut("{id:guid}/items/{itemId:guid}/dispense")]
    [Authorize(Roles = "Pharmacist,PharmacyAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DispenseItem(Guid id, Guid itemId, CancellationToken cancellationToken)
    {
        var existing = await _prescriptionService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(User, existing, "PrescriptionOwnerOrAdmin");
        if (!auth.Succeeded)
            return Forbid();

        var dispenserUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(dispenserUserId, out var dispenserId))
            throw new UnauthorizedAccessException("Invalid user ID.");
        await _prescriptionService.DispenseItemAsync(id, itemId, dispenserId, cancellationToken);
        return Ok(new { Message = "Prescription item dispensed successfully.", PrescriptionId = id, ItemId = itemId });
    }

    [HttpPut("{id:guid}/cancel")]
    [Authorize(Roles = "Doctor,Patient,HospitalAdmin,SuperAdmin,Pharmacist,PharmacyAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var existing = await _prescriptionService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound();

        var auth = await _authorizationService.AuthorizeAsync(User, existing, "PrescriptionOwnerOrAdmin");
        if (!auth.Succeeded)
            return Forbid();

        await _prescriptionService.CancelAsync(id, cancellationToken);
        return Ok(new { Message = "Prescription cancelled successfully.", PrescriptionId = id });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "HospitalAdmin,SuperAdmin,PharmacyAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _prescriptionService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}/hard-delete")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> HardDelete(Guid id, CancellationToken cancellationToken)
    {
        await _prescriptionService.HardDeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("deleted")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,PharmacyAdmin")]
    [EnableRateLimiting("BigDataLimiter")]
    public async Task<IActionResult> GetDeleted(CancellationToken cancellationToken)
    {
        var result = await _prescriptionService.GetDeletedAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/restore")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,PharmacyAdmin")]
    [EnableRateLimiting("SmallDataLimiter")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        await _prescriptionService.RestorePrescriptionAsync(id, cancellationToken);
        return Ok(new { Message = "Record restored successfully." });
    }


}