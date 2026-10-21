using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MediCore.Application.DTOs.Labs;
using MediCore.Application.Interfaces.Services;

namespace MediCore.API.Controllers;

[ApiController]
[Route("api/lab-tests")]
public class LabTestController : ControllerBase
{
    readonly ILabTestService _labTestService;

    public LabTestController(ILabTestService labTestService)
    {
        _labTestService = labTestService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LabTestDto>>> GetAll(CancellationToken cancellationToken)
    {
        var tests = await _labTestService.GetAllAsync(cancellationToken);
        return Ok(tests);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LabTestDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var test = await _labTestService.GetByIdAsync(id, cancellationToken);

        if (test is null)
            return NotFound();

        return Ok(test);
    }

    [HttpGet("by-name")]
    public async Task<ActionResult<LabTestDto>> GetByName([FromQuery] string name, CancellationToken cancellationToken)
    {
        var test = await _labTestService.GetByNameAsync(name, cancellationToken);

        if (test is null)
            return NotFound();

        return Ok(test);
    }

    [HttpPost]
    public async Task<ActionResult<LabTestDto>> Create(CreateLabTestDto dto, CancellationToken cancellationToken)
    {
        var test = await _labTestService.CreateAsync(dto, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = test.Id }, test);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<LabTestDto>> Update(Guid id, UpdateLabTestDto dto, CancellationToken cancellationToken)
    {
        var test = await _labTestService.UpdateAsync(id, dto, cancellationToken);
        return Ok(test);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _labTestService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}/hard-delete")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> HardDelete(Guid id, CancellationToken cancellationToken)
    {
        await _labTestService.HardDeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("deleted")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
    [EnableRateLimiting("BigDataLimiter")]
    public async Task<IActionResult> GetDeleted(CancellationToken cancellationToken)
    {
        var result = await _labTestService.GetDeletedLabTestsAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/restore")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
    [EnableRateLimiting("SmallDataLimiter")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        await _labTestService.RestoreLabTestAsync(id, cancellationToken);
        return Ok(new { Message = "Record restored successfully." });
    }

}