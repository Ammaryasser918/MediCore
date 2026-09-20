using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PulseID.Application.DTOs.ChronicDiseases;
using PulseID.Application.Interfaces.IServices;


namespace PulseID.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChronicDiseasesController : ControllerBase
    {
        readonly IChronicDiseaseService _chronicDiseaseService;

        public ChronicDiseasesController(IChronicDiseaseService chronicDiseaseService)
        {
            _chronicDiseaseService = chronicDiseaseService;
        }


        // GET: api/chronicdiseases
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        [EnableRateLimiting("BigDataLimiter")]
        public async Task<ActionResult<IReadOnlyList<ChronicDiseaseDto>>> GetAll(CancellationToken cancellationToken)
        {
            var diseases = await _chronicDiseaseService.GetAllAsync(cancellationToken);
            return Ok(diseases);
        }



        // GET: api/chronicdiseases/{id}
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        [EnableRateLimiting("SmallDataLimiter")]
        public async Task<ActionResult<ChronicDiseaseDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var disease = await _chronicDiseaseService.GetByIdAsync(id, cancellationToken);
            if (disease is null)
                return NotFound();

            return Ok(disease);
        }

        // GET: api/chronicdiseases/search?term=...
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        public async Task<ActionResult<IReadOnlyList<ChronicDiseaseDto>>> Search([FromQuery] string term, CancellationToken cancellationToken)
        {
            var diseases = await _chronicDiseaseService.SearchAsync(term, cancellationToken);
            return Ok(diseases);
        }


        // POST: api/chronicdiseases
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "HospitalAdmin,SuperAdmin")]
        public async Task<ActionResult<ChronicDiseaseDto>> Create([FromBody] CreateChronicDiseaseDto dto, CancellationToken cancellationToken)
        {
            var created = await _chronicDiseaseService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

    }
}
