using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediCore.Application.DTOs.PharmacyAdmin;
using MediCore.Application.Interfaces.IServices;
using MediCore.Domain.Common;

namespace MediCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public class PharmacyAdminController : ControllerBase
    {
        private readonly IPharmacyAdminService _pharmacyAdminService;
        public PharmacyAdminController(IPharmacyAdminService pharmacyAdminService)
        {
            _pharmacyAdminService = pharmacyAdminService;
        }


        [HttpGet]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var admin = await _pharmacyAdminService.GetByIdAsync(id, cancellationToken);
            return Ok(admin);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePharmacyAdminDto dto, CancellationToken cancellationToken)
        {
            var admin = await _pharmacyAdminService.CreatePharmacyAdminAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = admin.PharmacyId }, admin);
        }



    }
}
