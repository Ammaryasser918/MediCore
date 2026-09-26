using Microsoft.AspNetCore.Mvc;
using PulseID.Application.DTOs.Labs;
using PulseID.Application.Interfaces.Services;

namespace PulseID.API.Controllers;

[ApiController]
[Route("api/lab-technicians")]
public class LabTechnicianController : ControllerBase
{
    readonly ILabTechnicianService _labTechnicianService;

    public LabTechnicianController(ILabTechnicianService labTechnicianService)
    {
        _labTechnicianService = labTechnicianService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LabTechnicianDto>>> GetAll(CancellationToken cancellationToken)
    {
        var technicians = await _labTechnicianService.GetAllAsync(cancellationToken);
        return Ok(technicians);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LabTechnicianDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var technician = await _labTechnicianService.GetByIdAsync(id, cancellationToken);

        if (technician is null)
            return NotFound();

        return Ok(technician);
    }

    [HttpGet("by-lab/{labId:guid}")]
    public async Task<ActionResult<IReadOnlyList<LabTechnicianDto>>> GetByLab(Guid labId, CancellationToken cancellationToken)
    {
        var technicians = await _labTechnicianService.GetByLabAsync(labId, cancellationToken);
        return Ok(technicians);
    }

    [HttpPost]
    public async Task<ActionResult<LabTechnicianDto>> Create(CreateLabTechnicianDto dto, CancellationToken cancellationToken)
    {
        var technician = await _labTechnicianService.CreateAsync(dto, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = technician.Id }, technician);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateLabTechnicianDto dto, CancellationToken cancellationToken)
    {
        await _labTechnicianService.UpdateAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/transfer/{labId:guid}")]
    public async Task<IActionResult> TransferToLab(Guid id, Guid labId, CancellationToken cancellationToken)
    {
        await _labTechnicianService.TransferToLabAsync(id, labId, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] Guid hospitalAdminId, CancellationToken cancellationToken)
    {
        await _labTechnicianService.ApproveAsync(id, hospitalAdminId, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, CancellationToken cancellationToken)
    {
        await _labTechnicianService.RejectAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await _labTechnicianService.DeactivateAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/reactivate")]
    public async Task<IActionResult> Reactivate(Guid id, CancellationToken cancellationToken)
    {
        await _labTechnicianService.ReactivateAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _labTechnicianService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}