using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MediCore.Application.DTOs.Department;
using MediCore.Application.DTOs.Doctor;
using MediCore.Application.Interfaces.IServices;
using MediCore.Application.Services;

namespace MediCore.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DepartmentController : ControllerBase
{
    readonly IDepartmentService _departmentService;

    public DepartmentController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    // GET: api/department
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,PharmacyAdmin,LabAdmin,Doctor,Patient,Pharmacist,LabTechnician")]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetAll(CancellationToken cancellationToken)
    {
        var departments = await _departmentService.GetAllAsync(cancellationToken);

        return Ok(departments);
    }

    // GET: api/department/including-deleted
    [HttpGet("including-deleted")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetAllIncludingDeleted(CancellationToken cancellationToken)
    {
        var departments = await _departmentService.GetAllIncludingDeletedAsync(cancellationToken);

        return Ok(departments);
    }

    // GET: api/department/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,PharmacyAdmin,LabAdmin,Doctor,Patient,Pharmacist,LabTechnician")]
    public async Task<ActionResult<DepartmentDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var department = await _departmentService.GetByIdAsync(id, cancellationToken);

        return Ok(department);
    }

    // GET: api/department/{id}/details
    [HttpGet("{id:guid}/details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,PharmacyAdmin,LabAdmin,Doctor,Patient,Pharmacist,LabTechnician")]
    public async Task<ActionResult<DepartmentDto>> GetDetails(Guid id, CancellationToken cancellationToken)
    {
        var department = await _departmentService.GetDetailsAsync(id, cancellationToken);

        return Ok(department);
    }

    // GET: api/department/{id}/doctors
    [HttpGet("{id:guid}/doctors")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,PharmacyAdmin,LabAdmin,Doctor,Patient,Pharmacist,LabTechnician")]
    public async Task<ActionResult<IReadOnlyList<DoctorDto>>> GetDoctors(Guid id, CancellationToken cancellationToken)
    {
        var doctors = await _departmentService.GetDoctorsAsync(id, cancellationToken);

        return Ok(doctors);
    }

    // GET: api/department/hospital/{hospitalId}
    [HttpGet("hospital/{hospitalId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,PharmacyAdmin,LabAdmin,Doctor,Patient,Pharmacist,LabTechnician")]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetByHospital(Guid hospitalId, CancellationToken cancellationToken)
    {
        var departments = await _departmentService.GetByHospitalAsync(hospitalId, cancellationToken);

        return Ok(departments);
    }

    // GET: api/department/hospital/{hospitalId}/including-deleted
    [HttpGet("hospital/{hospitalId:guid}/including-deleted")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetByHospitalIncludingDeleted(Guid hospitalId, CancellationToken cancellationToken)
    {
        var departments = await _departmentService.GetByHospitalIncludingDeletedAsync(hospitalId, cancellationToken);

        return Ok(departments);
    }

    // POST: api/department/hospital/{hospitalId}
    [HttpPost("hospital/{hospitalId:guid}")]
    [Authorize(Roles = "HospitalAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DepartmentDto>> CreateDepartment(Guid hospitalId, CreateDepartmentDto dto, CancellationToken cancellationToken)
    {
        var department = await _departmentService.CreateDepartmentAsync(hospitalId, dto, cancellationToken);

        return Ok(department);
    }

    // PUT: api/department/{id}
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "HospitalAdmin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DepartmentDto>> Update(Guid id, [FromBody] UpdateDepartmentDto dto, CancellationToken cancellationToken)
    {
        var updated = await _departmentService.UpdateAsync(id, dto, cancellationToken);

        return Ok(updated);
    }

    // POST: api/department/{id}/doctors/{doctorId}
    [HttpPost("{id:guid}/doctors/{doctorId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "HospitalAdmin,SuperAdmin")]
    public async Task<ActionResult<DepartmentDto>> AssignDoctorToDepartment(Guid id, Guid doctorId, CancellationToken cancellationToken)
    {
        var updated = await _departmentService.AssignDoctorToDepartmentAsync(id, doctorId, cancellationToken);

        return Ok(updated);
    }

    // DELETE: api/department/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "HospitalAdmin,SuperAdmin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _departmentService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }

    [HttpDelete("hard-delete")]
    [Authorize("SuperAdmin")]
    public async Task<IActionResult> HardDelete(Guid id, CancellationToken cancellationToken)
    {
        await _departmentService.HardDeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("deleted")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
    [EnableRateLimiting("BigDataLimiter")]
    public async Task<IActionResult> GetDeleted(CancellationToken cancellationToken)
    {
        var result = await _departmentService.GetDeletedAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/restore")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
    [EnableRateLimiting("SmallDataLimiter")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        await _departmentService.RestoreDepartmentAsync(id, cancellationToken);
        return Ok(new { Message = "Record restored successfully." });
    }



}