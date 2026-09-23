using Microsoft.AspNetCore.Mvc;
using PulseID.Application.DTOs.Labs;
using PulseID.Application.Interfaces.Services;
using PulseID.Domain.Enums;

namespace PulseID.API.Controllers;

[ApiController]
[Route("api/lab-test-requests")]
public class LabTestRequestController : ControllerBase
{
    readonly ILabTestRequestService _labTestRequestService;

    public LabTestRequestController(ILabTestRequestService labTestRequestService)
    {
        _labTestRequestService = labTestRequestService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LabTestRequestDto>>> GetAll(CancellationToken cancellationToken)
    {
        var requests = await _labTestRequestService.GetAllAsync(cancellationToken);
        return Ok(requests);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LabTestRequestDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var request = await _labTestRequestService.GetByIdAsync(id, cancellationToken);

        if (request is null)
            return NotFound();

        return Ok(request);
    }

    [HttpGet("by-patient/{patientId:guid}")]
    public async Task<ActionResult<IReadOnlyList<LabTestRequestDto>>> GetByPatient(Guid patientId, CancellationToken cancellationToken)
    {
        var requests = await _labTestRequestService.GetByPatientAsync(patientId, cancellationToken);
        return Ok(requests);
    }

    [HttpGet("by-doctor/{doctorId:guid}")]
    public async Task<ActionResult<IReadOnlyList<LabTestRequestDto>>> GetByDoctor(Guid doctorId, CancellationToken cancellationToken)
    {
        var requests = await _labTestRequestService.GetByDoctorAsync(doctorId, cancellationToken);
        return Ok(requests);
    }

    [HttpGet("by-lab/{labId:guid}")]
    public async Task<ActionResult<IReadOnlyList<LabTestRequestDto>>> GetByLab(Guid labId, CancellationToken cancellationToken)
    {
        var requests = await _labTestRequestService.GetByLabAsync(labId, cancellationToken);
        return Ok(requests);
    }

    [HttpGet("by-status/{status}")]
    public async Task<ActionResult<IReadOnlyList<LabTestRequestDto>>> GetByStatus(LabTestRequestStatus status, CancellationToken cancellationToken)
    {
        var requests = await _labTestRequestService.GetByStatusAsync(status, cancellationToken);
        return Ok(requests);
    }

    [HttpPost]
    public async Task<ActionResult<LabTestRequestDto>> Create(CreateLabTestRequestDto dto, CancellationToken cancellationToken)
    {
        var request = await _labTestRequestService.CreateAsync(dto, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<LabTestRequestDto>> Update(Guid id, UpdateLabTestRequestDto dto, CancellationToken cancellationToken)
    {
        var request = await _labTestRequestService.UpdateAsync(id, dto, cancellationToken);
        return Ok(request);
    }

    [HttpPut("{id:guid}/assign-lab/{labId:guid}")]
    public async Task<IActionResult> AssignLab(Guid id, Guid labId, CancellationToken cancellationToken)
    {
        await _labTestRequestService.AssignLabAsync(id, labId, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/send-to-lab")]
    public async Task<IActionResult> SendToLab(Guid id, CancellationToken cancellationToken)
    {
        await _labTestRequestService.SendToLabAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/collect-sample")]
    public async Task<IActionResult> CollectSample(Guid id, CancellationToken cancellationToken)
    {
        await _labTestRequestService.CollectSampleAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/start-processing")]
    public async Task<IActionResult> StartProcessing(Guid id, CancellationToken cancellationToken)
    {
        await _labTestRequestService.StartProcessingAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/upload-result")]
    public async Task<IActionResult> UploadResult(Guid id, [FromBody] string resultFileUrl, CancellationToken cancellationToken)
    {
        await _labTestRequestService.UploadResultAsync(id, resultFileUrl, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/upload-patient-result")]
    public async Task<IActionResult> UploadPatientResult(Guid id, [FromBody] string resultFileUrl, CancellationToken cancellationToken)
    {
        await _labTestRequestService.UploadPatientResultAsync(id, resultFileUrl, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, CancellationToken cancellationToken)
    {
        await _labTestRequestService.RejectAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _labTestRequestService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}