using Microsoft.AspNetCore.Mvc;
using PulseID.Application.DTOs.Department;
using PulseID.Application.DTOs.Doctor;
using PulseID.Application.Interfaces.IServices;

namespace PulseID.API.Controllers;

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
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetAll(CancellationToken cancellationToken)
    {
        var departments = await _departmentService.GetAllAsync(cancellationToken);

        return Ok(departments);
    }

    // GET: api/department/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DepartmentDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var department = await _departmentService.GetByIdAsync(id, cancellationToken);

        return Ok(department);
    }

    [HttpPost("hospital/{hospitalId:guid}")]
    public async Task<ActionResult<DepartmentDto>> CreateDepartment(Guid id, CreateDepartmentDto dto, CancellationToken cancellationToken)
    {
        var department = await _departmentService.CreateDepartmentAsync(id, dto, cancellationToken);

        return Ok(department);
    }

    // PUT: api/department/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DepartmentDto>> Update(Guid id, [FromBody] UpdateDepartmentDto dto, CancellationToken cancellationToken)
    {
        var updated = await _departmentService.UpdateAsync(id, dto, cancellationToken);

        return Ok(updated);
    }

    // GET: api/department/{id}/doctors
    [HttpGet("{id:guid}/doctors")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<DoctorDto>>> GetDoctors(Guid id, CancellationToken cancellationToken)
    {
        var doctors = await _departmentService.GetDoctorsAsync(id, cancellationToken);

        return Ok(doctors);
    }

    // POST: api/department/{id}/doctors/{doctorId}
    [HttpPost("{id:guid}/doctors/{doctorId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DepartmentDto>> AssignDoctorToDepartment(Guid id, Guid doctorId, CancellationToken cancellationToken)
    {
        var updated = await _departmentService.AssignDoctorToDepartmentAsync(
            id,
            doctorId,
            cancellationToken);

        return Ok(updated);
    }
}