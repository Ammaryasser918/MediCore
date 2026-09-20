using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PulseID.Application.DTOs.Allergies;
using PulseID.Application.Interfaces.IServices;

namespace PulseID.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AllergiesController : ControllerBase
    {

        IAllergyService _allergyService;
        public AllergiesController(IAllergyService allergyService)
        {
            _allergyService = allergyService;
        }


        // GET: api/allergies
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        [EnableRateLimiting("BigDataLimiter")]
        public async Task<ActionResult<IReadOnlyList<AllergyDto>>> GetAll(CancellationToken cancellationToken)
        {
            var allergies = await _allergyService.GetAllAsync(cancellationToken);
            return Ok(allergies);
        }


        // GET: api/allergies/search?term=...
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        public async Task<ActionResult<IReadOnlyList<AllergyDto>>> Search([FromQuery] string term, CancellationToken cancellationToken)
        {
            var allergies = await _allergyService.SearchAsync(term, cancellationToken);
            if (allergies == null) 
                return NotFound();
            return Ok(allergies);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        [EnableRateLimiting("SmallDataLimiter")]
        public async Task<ActionResult<AllergyDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var allergy = await _allergyService.GetByIdAsync(id, cancellationToken);
            if (allergy == null)
                return NotFound();
            return Ok(allergy);
        }


        // POST: api/allergies
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "HospitalAdmin,SuperAdmin")]
        public async Task<ActionResult<AllergyDto>> Create([FromBody] CreateAllergyDto dto, CancellationToken cancellationToken)
        {
            var created = await _allergyService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }


    }
}
