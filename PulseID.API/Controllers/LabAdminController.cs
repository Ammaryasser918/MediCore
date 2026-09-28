using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PulseID.Application.DTOs.LabAdminDto;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Common;

namespace PulseID.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public class LabAdminController : ControllerBase
    {

        ILabAdminService _labAdminService;

        public LabAdminController(ILabAdminService labAdminService)
        {
            _labAdminService = labAdminService;
        }

        [HttpGet]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var admin = await _labAdminService.GetById(id, cancellationToken);
            return Ok(admin);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLabAdminDto dto, CancellationToken cancellationToken= default)
        {
            var admin = await _labAdminService.CreateLabAdminAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { idd = admin.Id }, admin);
        }

    }
}
