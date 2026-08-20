using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PulseID.Application.DTOs.Doctor;
using PulseID.Application.Interfaces.IServices;

namespace PulseID.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }


        // GET: api/doctors
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<DoctorDto>>> GetAll(CancellationToken cancellationToken)
        {
            var doctors = await _doctorService.GetAllAsync(cancellationToken);
            return Ok(doctors);
        }

        // GET: api/doctors/{id}
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<DoctorDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var doctor = await _doctorService.GetByIdAsync(id, cancellationToken);
            return Ok(doctor);
        }

        // GET: api/doctors/by-department/{departmentId}
        [HttpGet("by-department/{departmentId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<DoctorDto>>> GetByDepartment(Guid departmentId, CancellationToken cancellationToken)
        {
            var doctors = await _doctorService.GetByDepartmentAsync(departmentId, cancellationToken);
            return Ok(doctors);
        }


        // POST: api/doctors
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DoctorDto>> Create([FromBody] CreateDoctorDto dto, CancellationToken cancellationToken)
        {
            var created = await _doctorService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }


        // PUT: api/doctors/{id}/department
        [HttpPut("{id:guid}/department")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorDto>> UpdateDepartment(Guid id, [FromBody] UpdateDoctorDepartmentDto dto, CancellationToken cancellationToken)
        {
            var updated = await _doctorService.UpdateDepartmentAsync(id, dto, cancellationToken);
            return Ok(updated);
        }

        // PUT: api/doctors/{id}
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DoctorDto>> Update(Guid id, [FromBody] UpdateDoctorDto dto, CancellationToken cancellationToken)
        {
            var updated = await _doctorService.UpdateAsync(id, dto, cancellationToken);
            return Ok(updated);
        }


        // DELETE: api/doctors/{id}
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _doctorService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        // PATCH: api/doctors/{id}/approve?hospitalAdminId=...
        [HttpPatch("{id:guid}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorDto>> Approve(Guid id, [FromQuery] Guid hospitalAdminId, CancellationToken cancellationToken)
        {
            var updated = await _doctorService.ApproveAsync(id, hospitalAdminId, cancellationToken);
            return Ok(updated);
        }



        // PATCH: api/doctors/{id}/reject
        [HttpPatch("{id:guid}/reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorDto>> Reject(Guid id, [FromQuery] UpdateDoctorDepartmentDto dto, CancellationToken cancellationToken)
        {
            var updated = await _doctorService.RejectAsync(id, cancellationToken);
            return Ok(updated);
        }


        // PATCH: api/doctors/{id}/deactivate
        [HttpPatch("{id:guid}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorDto>> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var updated = await _doctorService.DeactivateAsync(id, cancellationToken);
            return Ok(updated);
        }


        // PATCH: api/doctors/{id}/reactivate
        [HttpPatch("{id:guid}/reactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorDto>> Reactivate(Guid id, CancellationToken cancellationToken)
        {
            var updated = await _doctorService.ReactivateAsync(id, cancellationToken);
            return Ok(updated);
        }

    }
}
