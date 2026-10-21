using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MediCore.Application.DTOs.Labs;
using MediCore.Application.Interfaces.IServices;
using MediCore.Application.Interfaces.Services;
using MediCore.Domain.Enums;

namespace MediCore.API.Controllers;

[ApiController]
[Route("api/lab-test-requests")]
public class LabTestRequestController : ControllerBase
{
    readonly ILabTestRequestService _labTestRequestService;
    readonly IAuthorizationService _authorizationService;
    readonly IPatientService _patientService;
    readonly IDoctorService _doctorService;

    public LabTestRequestController(ILabTestRequestService labTestRequestService, IAuthorizationService authorizationService, 
        IPatientService patientService, IDoctorService doctorService)
    {
        _labTestRequestService = labTestRequestService;
        _authorizationService = authorizationService;
        _patientService = patientService;
        _doctorService = doctorService;
    }

    [HttpGet]
    [Authorize(Roles = "LabAdmin,HospitalAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [EnableRateLimiting("BigDataLimiter")]
    public async Task<ActionResult<IReadOnlyList<LabTestRequestDto>>> GetAll(CancellationToken cancellationToken)
    {
        var requests = await _labTestRequestService.GetAllAsync(cancellationToken);
        return Ok(requests);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Patient,Doctor,LabTechnician,LabAdmin,HospitalAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [EnableRateLimiting("SmallDataLimiter")]
    public async Task<ActionResult<LabTestRequestDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var request = await _labTestRequestService.GetByIdAsync(id, cancellationToken);

        if (request is null)
            return NotFound();

        return Ok(request);
    }

    [HttpGet("by-patient/{patientId:guid}")]
    [Authorize(Roles = "Patient,Doctor,LabAdmin,HospitalAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [EnableRateLimiting("BigDataLimiter")]
    public async Task<ActionResult<IReadOnlyList<LabTestRequestDto>>> GetByPatient(Guid patientId, CancellationToken cancellationToken)
    {
        var requests = await _labTestRequestService.GetByPatientAsync(patientId, cancellationToken);
        return Ok(requests);
    }

    [HttpGet("by-doctor/{doctorId:guid}")]
    [Authorize(Roles = "Doctor,HospitalAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [EnableRateLimiting("BigDataLimiter")]
    public async Task<ActionResult<IReadOnlyList<LabTestRequestDto>>> GetByDoctor(Guid doctorId, CancellationToken cancellationToken)
    {
        var requests = await _labTestRequestService.GetByDoctorAsync(doctorId, cancellationToken);
        return Ok(requests);
    }

    [HttpGet("by-lab/{labId:guid}")]
    [Authorize(Roles = "LabTechnician,LabAdmin,HospitalAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [EnableRateLimiting("BigDataLimiter")]
    public async Task<ActionResult<IReadOnlyList<LabTestRequestDto>>> GetByLab(Guid labId, CancellationToken cancellationToken)
    {
        var requests = await _labTestRequestService.GetByLabAsync(labId, cancellationToken);
        return Ok(requests);
    }

    [HttpGet("by-status/{status}")]
    [Authorize(Roles = "LabTechnician,LabAdmin,HospitalAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [EnableRateLimiting("BigDataLimiter")]
    public async Task<ActionResult<IReadOnlyList<LabTestRequestDto>>> GetByStatus(LabTestRequestStatus status, CancellationToken cancellationToken)
    {
        var requests = await _labTestRequestService.GetByStatusAsync(status, cancellationToken);
        return Ok(requests);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Doctor,SuperAdmin")]
    public async Task<ActionResult<LabTestRequestDto>> Create(CreateLabTestRequestDto dto, CancellationToken cancellationToken)
    {
        var request = await _labTestRequestService.CreateAsync(dto, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Doctor,HospitalAdmin,SuperAdmin")]
    public async Task<ActionResult<LabTestRequestDto>> Update(Guid id, UpdateLabTestRequestDto dto, CancellationToken cancellationToken)
    {
        var request = await _labTestRequestService.UpdateAsync(id, dto, cancellationToken);
        return Ok(request);
    }

    [HttpPut("{id:guid}/assign-lab/{labId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Doctor,LabAdmin,HospitalAdmin,SuperAdmin")]
    public async Task<IActionResult> AssignLab(Guid id, Guid labId, CancellationToken cancellationToken)
    {
        await _labTestRequestService.AssignLabAsync(id, labId, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/send-to-lab")]
    [Authorize(Roles = "Doctor,LabAdmin,HospitalAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendToLab(Guid id, CancellationToken cancellationToken)
    {
        await _labTestRequestService.SendToLabAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/collect-sample")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "LabTechnician,LabAdmin,SuperAdmin")]
    public async Task<IActionResult> CollectSample(Guid id, CancellationToken cancellationToken)
    {
        await _labTestRequestService.CollectSampleAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/start-processing")]
    [Authorize(Roles = "LabTechnician,LabAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartProcessing(Guid id, CancellationToken cancellationToken)
    {
        await _labTestRequestService.StartProcessingAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/upload-result")]
    [Authorize(Roles = "LabTechnician,LabAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadResult(Guid id, [FromBody] string resultFileUrl, CancellationToken cancellationToken)
    {
        await _labTestRequestService.UploadResultAsync(id, resultFileUrl, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/upload-patient-result")]
    [Authorize(Roles = "LabTechnician,LabAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadPatientResult(Guid id, [FromBody] string resultFileUrl, CancellationToken cancellationToken)
    {
        await _labTestRequestService.UploadPatientResultAsync(id, resultFileUrl, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/reject")]
    [Authorize(Roles = "LabTechnician,LabAdmin,HospitalAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(Guid id, CancellationToken cancellationToken)
    {
        await _labTestRequestService.RejectAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "HospitalAdmin,SuperAdmin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _labTestRequestService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}