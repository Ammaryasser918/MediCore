using Microsoft.AspNetCore.Mvc;
using PulseID.Application.DTOs.Labs;
using PulseID.Application.Interfaces.Services;

namespace PulseID.API.Controllers;

[ApiController]
[Route("api/labs")]
public class LabController : ControllerBase
{
    readonly ILabService _labService;

    public LabController(ILabService labService)
    {
        _labService = labService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LabDto>>> GetAll(CancellationToken cancellationToken)
    {
        var labs = await _labService.GetAllAsync(cancellationToken);
        return Ok(labs);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LabDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var lab = await _labService.GetByIdAsync(id, cancellationToken);

        if (lab is null)
            return NotFound();

        return Ok(lab);
    }

    [HttpGet("{id:guid}/requests")]
    public async Task<ActionResult<LabDto>> GetWithRequests(Guid id, CancellationToken cancellationToken)
    {
        var lab = await _labService.GetWithRequestsAsync(id, cancellationToken);

        if (lab is null)
            return NotFound();

        return Ok(lab);
    }

    [HttpPost]
    public async Task<ActionResult<LabDto>> Create(CreateLabDto dto, CancellationToken cancellationToken)
    {
        var lab = await _labService.CreateAsync(dto, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = lab.Id }, lab);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<LabDto>> Update(Guid id, UpdateLabDto dto, CancellationToken cancellationToken)
    {
        var lab = await _labService.UpdateAsync(id, dto, cancellationToken);
        return Ok(lab);
    }

    [HttpPut("{id:guid}/license-document")]
    public async Task<IActionResult> UpdateLicenseDocument(Guid id, [FromBody] string documentPath, CancellationToken cancellationToken)
    {
        await _labService.UpdateLicenseDocumentAsync(id, documentPath, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] Guid hospitalAdminId, CancellationToken cancellationToken)
    {
        await _labService.ApproveAsync(id, hospitalAdminId, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, CancellationToken cancellationToken)
    {
        await _labService.RejectAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await _labService.DeactivateAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/reactivate")]
    public async Task<IActionResult> Reactivate(Guid id, CancellationToken cancellationToken)
    {
        await _labService.ReactivateAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _labService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}