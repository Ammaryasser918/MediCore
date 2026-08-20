using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PulseID.Application.DTOs.Doctor;
using PulseID.Application.DTOs.Pharmacist;
using PulseID.Application.Interfaces.IServices;
using PulseID.Application.Services;

namespace PulseID.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacistController : ControllerBase
    {
        readonly IPharmacistService _pharmacistService;

        public PharmacistController(IPharmacistService pharmacistService)
        {
            _pharmacistService = pharmacistService;
        }



        // GET: api/pharmacists
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<PharmacistDto>>> GetAll(CancellationToken cancellationToken)
        {
            var pharmacists = await _pharmacistService.GetAllAsync(cancellationToken);
            return Ok(pharmacists);
        }


        // GET: api/pharmacists/{id}
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PharmacistDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var pharmacist = await _pharmacistService.GetByIdAsync(id, cancellationToken);
            return Ok(pharmacist);
        }

        //POST: api/pharmacists
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PharmacistDto>> Create([FromBody] CreatePharmacistDto dto, CancellationToken cancellationToken)
        {
            var created = await _pharmacistService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/pharmacists/{id}/pharmacy
        [HttpPut("{id:guid}/pharmacy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PharmacistDto>> AssignToPharmacy(Guid id, [FromBody] AssignPharmacistToPharmacyDto dto, CancellationToken cancellationToken)
        {
            var updated = await _pharmacistService.AssignToPharmacyAsync(id, dto, cancellationToken);
            return Ok(updated);
        }

        // PUT: api/pharmacists/{id}
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PharmacistDto>> Update(Guid id, [FromBody] UpdatePharmacistDto dto, CancellationToken cancellationToken)
        {
            var updated = await _pharmacistService.UpdatePharmacistDetailsAsync(id, dto, cancellationToken);
            return Ok(updated);
        }


        // DELETE: api/Pharmacists/{id}
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _pharmacistService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        // PATCH: api/pharmacists/{id}/approve?pharmacyAdminId=...
        [HttpPatch("{id:guid}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PharmacistDto>> Approve(Guid id, [FromQuery] Guid pharmacyAdminId, CancellationToken cancellationToken)
        {
            var updated = await _pharmacistService.ApproveAsync(id, pharmacyAdminId, cancellationToken);
            return Ok(updated);
        }


        // PATCH: api/pharmacists/{id}/reject
        [HttpPatch("{id:guid}/reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PharmacistDto>> Reject(Guid id, CancellationToken cancellationToken)
        {
            var updated = await _pharmacistService.RejectAsync(id, cancellationToken);
            return Ok(updated);
        }

        // PATCH: api/pharmacists/{id}/deactivate
        [HttpPatch("{id:guid}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PharmacistDto>> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var updated = await _pharmacistService.DeactivateAsync(id, cancellationToken);
            return Ok(updated);
        }

        // PATCH: api/pharmacists/{id}/reactivate
        [HttpPatch("{id:guid}/reactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PharmacistDto>> Reactivate(Guid id, CancellationToken cancellationToken)
        {
            var updated = await _pharmacistService.ReactivateAsync(id, cancellationToken);
            return Ok(updated);
        }




    }
}
