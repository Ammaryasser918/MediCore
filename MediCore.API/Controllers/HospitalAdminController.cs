using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediCore.Application.DTOs.HospitalAdmin;
using MediCore.Application.Interfaces.IServices;
using MediCore.Domain.Common;
using System.Diagnostics.Contracts;

namespace MediCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public class HospitalAdminController : ControllerBase
    {
        private readonly IHospitalAdminService _hospitalAdminService;

        public HospitalAdminController(IHospitalAdminService hospitalAdminService)
        {
            _hospitalAdminService = hospitalAdminService;
        }

        [HttpGet]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var admin = await _hospitalAdminService.GetById(id, cancellationToken);
            return Ok(admin);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateHospitalAdminDto dto, CancellationToken cancellationToken)
        {
            var admin = await _hospitalAdminService.CreateHospitalAdminAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = admin.HospitalId }, admin);
        }


    }
}
