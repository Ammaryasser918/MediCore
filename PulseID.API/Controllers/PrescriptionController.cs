using Microsoft.AspNetCore.Mvc;
using PulseID.Application.DTOs.Prescriptions;
using PulseID.Application.Interfaces.Services;
using PulseID.Domain.Enums;

namespace PulseID.API.Controllers;

[ApiController]
[Route("api/prescriptions")]
public class PrescriptionController : ControllerBase
{
    readonly IPrescriptionService _prescriptionService;

    public PrescriptionController(IPrescriptionService prescriptionService)
    {
        _prescriptionService = prescriptionService;
    }

    [HttpGet("by-medical-record/{medicalRecordId:guid}")]
    public async Task<ActionResult<IReadOnlyList<PrescriptionDto>>> GetByMedicalRecord(Guid medicalRecordId, CancellationToken cancellationToken)
    {
        var prescriptions = await _prescriptionService.GetByMedicalRecordAsync(medicalRecordId, cancellationToken);
        return Ok(prescriptions);
    }

    [HttpGet("by-status/{status}")]
    public async Task<ActionResult<IReadOnlyList<PrescriptionDto>>> GetByStatus(PrescriptionStatus status, CancellationToken cancellationToken)
    {
        var prescriptions = await _prescriptionService.GetByStatusAsync(status, cancellationToken);
        return Ok(prescriptions);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PrescriptionDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var prescription = await _prescriptionService.GetByIdAsync(id, cancellationToken);

        if (prescription is null)
            return NotFound();

        return Ok(prescription);
    }

    [HttpGet("{id:guid}/items")]
    public async Task<ActionResult<PrescriptionDto>> GetWithItems(Guid id, CancellationToken cancellationToken)
    {
        var prescription = await _prescriptionService.GetWithItemsAsync(id, cancellationToken);

        if (prescription is null)
            return NotFound();

        return Ok(prescription);
    }

    [HttpPost]
    public async Task<ActionResult<PrescriptionDto>> Create(CreatePrescriptionDto dto, CancellationToken cancellationToken)
    {
        var prescription = await _prescriptionService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = prescription.Id }, prescription);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PrescriptionDto>> Update(Guid id, UpdatePrescriptionDto dto, CancellationToken cancellationToken)
    {
        var prescription = await _prescriptionService.UpdateAsync(id, dto, cancellationToken);
        return Ok(prescription);
    }

    [HttpPost("{id:guid}/items")]
    public async Task<IActionResult> AddItem(Guid id, AddPrescriptionItemDto dto, CancellationToken cancellationToken)
    {
        await _prescriptionService.AddItemAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> UpdateItem(Guid id, Guid itemId, UpdatePrescriptionItemDto dto, CancellationToken cancellationToken)
    {
        await _prescriptionService.UpdateItemAsync(id, itemId, dto, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid id, Guid itemId, CancellationToken cancellationToken)
    {
        await _prescriptionService.RemoveItemAsync(id, itemId, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/verify")]
    public async Task<IActionResult> Verify(Guid id, CancellationToken cancellationToken)
    {
        await _prescriptionService.VerifyAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/items/{itemId:guid}/dispense")]
    public async Task<IActionResult> DispenseItem(Guid id, Guid itemId, [FromBody] Guid pharmacistId, CancellationToken cancellationToken)
    {
        await _prescriptionService.DispenseItemAsync(id, itemId, pharmacistId, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        await _prescriptionService.CancelAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _prescriptionService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}