using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PulseID.API.Authorization;
using PulseID.Application.DTOs.Doctor;
using PulseID.Application.Interfaces.IServices;
using System.Security.Claims;

namespace PulseID.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        readonly IDoctorService _doctorService;
        readonly IAuthorizationService _authorizationService;

        public DoctorController(IDoctorService doctorService, IAuthorizationService authorizationService)
        {
            _doctorService = doctorService;
            _authorizationService = authorizationService;
        }


        // GET: api/doctor
        [Authorize(Roles = "HospitalAdmin,SuperAdmin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<DoctorDto>>> GetAll(CancellationToken cancellationToken)
        {
            var doctors = await _doctorService.GetAllAsync(cancellationToken);
            return Ok(doctors);
        }

        // GET: api/doctor/{id}
        [Authorize(Roles = "HospitalAdmin,Doctor,SuperAdmin")]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<DoctorDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var doctor = await _doctorService.GetByIdAsync(id, cancellationToken);
            if (doctor is null) return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, doctor, "DoctorOwnerOrAdmin");
            if (!authResult.Succeeded)
                return Forbid();

            return Ok(doctor);
        }

        // GET: api/doctor/by-department/{departmentId}
        [Authorize(Roles = "HospitalAdmin,SuperAdmin")]
        [HttpGet("by-department/{departmentId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<DoctorDto>>> GetByDepartment(Guid departmentId, CancellationToken cancellationToken)
        {
            var doctors = await _doctorService.GetByDepartmentAsync(departmentId, cancellationToken);
            return Ok(doctors);
        }


        // POST: api/doctor
        [Authorize(Roles = "HospitalAdmin,SuperAdmin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DoctorDto>> Create([FromBody] CreateDoctorDto dto, CancellationToken cancellationToken)
        {
            var created = await _doctorService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }


        // PUT: api/doctor/{id}/department
        [Authorize(Roles = "HospitalAdmin,SuperAdmin")]
        [HttpPut("{id:guid}/department")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorDto>> UpdateDepartment(Guid id, [FromBody] UpdateDoctorDepartmentDto dto, CancellationToken cancellationToken)
        {
            var updated = await _doctorService.UpdateDepartmentAsync(id, dto, cancellationToken);
            return Ok(updated);
        }

        // PUT: api/doctor/{id}
        [Authorize(Roles = "HospitalAdmin,SuperAdmin")]
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DoctorDto>> Update(Guid id, [FromBody] UpdateDoctorDto dto, CancellationToken cancellationToken)
        {

            var existingDoctor = await _doctorService.GetByIdAsync(id, cancellationToken);
            if (existingDoctor is null) return NotFound();

            var authService = await _authorizationService.AuthorizeAsync(User, existingDoctor, "DoctorOwnerOrAdmin");
            if (!authService.Succeeded)
                return Forbid();

            var updated = await _doctorService.UpdateAsync(id, dto, cancellationToken);
            return Ok(updated);
        }


        // DELETE: api/doctor/{id}
        [Authorize(Roles = "HospitalAdmin,SuperAdmin")]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _doctorService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        // PATCH: api/doctor/{id}/approve?hospitalAdminId=...
        [Authorize(Roles = "HospitalAdmin,SuperAdmin")]
        [HttpPatch("{id:guid}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorDto>> Approve(Guid id, CancellationToken cancellationToken)
        {
            var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(adminIdClaim, out var hospitalAdminId))
                return Unauthorized("Can't identify the current admin user.");

            var updated = await _doctorService.ApproveAsync(id, hospitalAdminId, cancellationToken);
            return Ok(updated);
        }



        // PATCH: api/doctor/{id}/reject
        [Authorize(Roles = "HospitalAdmin,SuperAdmin")]
        [HttpPatch("{id:guid}/reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorDto>> Reject(Guid id, [FromQuery] UpdateDoctorDepartmentDto dto, CancellationToken cancellationToken)
        {
            var updated = await _doctorService.RejectAsync(id, cancellationToken);
            return Ok(updated);
        }


        // PATCH: api/doctor/{id}/deactivate
        [Authorize(Roles = "HospitalAdmin,SuperAdmin")]
        [HttpPatch("{id:guid}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorDto>> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var updated = await _doctorService.DeactivateAsync(id, cancellationToken);
            return Ok(updated);
        }


        // PATCH: api/doctor/{id}/reactivate
        [Authorize(Roles = "HospitalAdmin,SuperAdmin")]
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
