using Microsoft.AspNetCore.Mvc;
using PulseID.Application.DTOs.Medications;
using PulseID.Application.Interfaces.Services;

namespace PulseID.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicationController : ControllerBase
{
    readonly IMedicationService _medicationService;

    public MedicationController(IMedicationService medicationService)
    {
        _medicationService = medicationService;
    }

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<MedicationDto>>> Search([FromQuery] string searchTerm, CancellationToken cancellationToken)
    {
        var medications = await _medicationService.SearchAsync(searchTerm, cancellationToken);
        return Ok(medications);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MedicationDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var medication = await _medicationService.GetByIdAsync(id, cancellationToken);

        if (medication is null)
            return NotFound();

        return Ok(medication);
    }

    [HttpPost]
    public async Task<ActionResult<MedicationDto>> Create(CreateMedicationDto dto, CancellationToken cancellationToken)
    {
        var medication = await _medicationService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = medication.Id }, medication);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<MedicationDto>> Update(Guid id, UpdateMedicationDto dto, CancellationToken cancellationToken)
    {
        var medication = await _medicationService.UpdateAsync(id, dto, cancellationToken);
        return Ok(medication);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _medicationService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}