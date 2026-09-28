using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration.UserSecrets;
using PulseID.Application.DTOs.Auth;
using PulseID.Application.DTOs.OTP;
using PulseID.Application.Interfaces.IServices;

using System.Diagnostics.Contracts;
using System.Security.Claims;


namespace PulseID.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        // ------- Patient Registration --------

        // POST: api/auth/patient/register/request-otp
        [HttpPost("patient/register/request-otp")]
        [EnableRateLimiting("AuthLimiter")]
        public async Task<IActionResult> RequestPatientRegisterationOtp([FromBody] RegisterPatientDto dto, CancellationToken cancellationToken)
        {
            var result = await _authService.RequestPatientRegistrationOtpAsync(dto, cancellationToken);
            return Ok(new 
            { 
                message = result.Message, 
                registrationToken = result.RegistrationToken,
                OTP = result.Otp
            });
        }

        // POST: api/auth/patient/register/verify
        [HttpPost("patient/register/verify")]
        [EnableRateLimiting("AuthLimiter")]
        public async Task<IActionResult> VerifyPatientRegisteration([FromBody] VerifyRegistrationDto dto, CancellationToken cancellationToken)
        {
            var result = await _authService.VerifyPatientRegistrationAsync(dto.RegistrationToken, dto.Otp, cancellationToken);
            if (!result.Success)
                return BadRequest(new { message = result.Error });
            return Ok(new { accessToken = result.AccessToken, refreshToken = result.RefreshToken});
        }



        // ------ Doctor Registration --------

        // POST: api/auth/doctor/register/request-otp
        [HttpPost("doctor/register/request-otp")]
        [EnableRateLimiting("AuthLimiter")]
        public async Task<IActionResult> RequestDoctorRegisterationOtp([FromBody] RegisterDoctorDto dto, CancellationToken cancellationToken)
        {
            var result = await _authService.RequestDoctorRegistrationOtpAsync(dto, cancellationToken);
            return Ok(new
            {
                message = result.Message,
                registrationToken = result.RegistrationToken,
                OTP = result.Otp
            });
        }


        // POST: api/auth/doctor/register/verify
        [HttpPost("doctor/register/verify")]
        [EnableRateLimiting("AuthLimiter")]
        public async Task<IActionResult> VerifyDoctorRegisteration([FromBody] VerifyRegistrationDto dto, CancellationToken cancellationToken)
        {
            var result = await _authService.VerifyDoctorRegistrationAsync(dto.RegistrationToken, dto.Otp, cancellationToken);
            if (!result.Success)
                return BadRequest(new { message = result.Error });
            return Ok(new { accessToken = result.AccessToken, refreshToken = result.RefreshToken});
        }


        // ------ Pharmacist Registration --------

        // POST: api/auth/pharmacist/register/request-otp
        [HttpPost("pharmacist/register/request-otp")]
        [EnableRateLimiting("AuthLimiter")]
        public async Task<IActionResult> RequestPharmacistRegisterationOtp([FromBody] RegisterPharmacistDto dto, CancellationToken cancellationToken)
        {
            var result = await _authService.RequestPharmacistRegistrationOtpAsync(dto, cancellationToken);
            return Ok(new
            {
                message = result.Message,
                registrationToken = result.RegistrationToken,
                OTP = result.Otp
            });
        }

        // POST: api/auth/pharmacist/register/verify
        [HttpPost("pharmacist/register/verify")]
        [EnableRateLimiting("AuthLimiter")]
        public async Task<IActionResult> VerifyPharmacistRegisteration([FromBody] VerifyRegistrationDto dto, CancellationToken cancellationToken)
        {
            var result = await _authService.VerifyPharmacistRegistrationAsync(dto.RegistrationToken, dto.Otp, cancellationToken);
            if (!result.Success)
                return BadRequest(new { message = result.Error });
            return Ok(new { accessToken = result.AccessToken, refreshToken = result.RefreshToken});
        }


        // ------ Lab Technician Registration --------

        // POST: api/auth/lab-technician/register/request-otp
        [HttpPost("lab-technician/register/request-otp")]
        [EnableRateLimiting("AuthLimiter")]
        public async Task<IActionResult> RequestLabTechnicianRegisterationOtp([FromBody] RegisterLabTechnicianDto dto, CancellationToken cancellationToken)
        {
            var result = await _authService.RequestLabTechnicianRegistrationOtpAsync(dto, cancellationToken);
            return Ok(new
            {
                message = result.Message,
                registrationToken = result.RegistrationToken,
                OTP = result.Otp
            });
        }

        // POST: api/auth/lab-technician/register/verify
        [HttpPost("lab-technician/register/verify")]
        [EnableRateLimiting("AuthLimiter")]
        public async Task<IActionResult> VerifyLabTechnicianRegisteration([FromBody] VerifyRegistrationDto dto, CancellationToken cancellationToken)
        {
            var result = await _authService.VerifyLabTechnicianRegistrationAsync(dto.RegistrationToken, dto.Otp, cancellationToken);
            if (!result.Success)
                return BadRequest(new { message = result.Error });
            return Ok(new { accessToken = result.AccessToken, refreshToken = result.RefreshToken});
        }


        // ------  Common  --------

        // POST: api/auth/login/request-otp
        [HttpPost("login/request-otp")]
        [EnableRateLimiting("AuthLimiter")]
        public async Task<IActionResult> RequestLoginOtp([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.RequestLoginOtpAsync(request.Email, request.Password, cancellationToken);
            return Ok(new
            {
                message = result.Message,
                OTP = result.Otp
            });
        }


        // POST: api/auth/login/verify
        [HttpPost("login/verify")]
        [EnableRateLimiting("AuthLimiter")]
        public async Task<IActionResult> VerifyLogin(VerifyLoginDto dto, CancellationToken cancellationToken)
        {
            var result = await _authService.VerifyLoginAsync(dto.Email, dto.Otp, cancellationToken);
            if (!result.Success)
                return BadRequest(new { message = result.Error });

            return Ok(new { accessToken = result.AccessToken, refreshToken = result.RefreshToken});
        }



        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();
            await _authService.LogoutAsync(userId, cancellationToken);
            return Ok(new { message = "Logged out successfully." });
        }

        
        [HttpPost("refresh")]
        [EnableRateLimiting("AuthLimiter")]
        public async Task<IActionResult> Refresh([FromBody] Application.DTOs.Auth.RefreshRequest request,  CancellationToken cancellationToken)
        {

            var result = await _authService.RefreshAsync(request.UserId, request.RefreshToken, cancellationToken);
            if (result is null) return Unauthorized("Invalid or expired refresh token.");

            return Ok(new { accessToken = result.AccessToken, refreshToken = result.RefreshToken });

        }
        
        [HttpGet("session")]
        public IActionResult GetSession()
        {

            if (User.Identity is null || !User.Identity.IsAuthenticated)
                return Ok(new { isAuthenticated = false, user = (object?)null });

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            
            return Ok(new
            {
                isAuthenticated = true,
                user = new { id = userId, email, role }
            });

        }

        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Error = "User is not authenticated." });
            }

            var result = await _authService.ChangePasswordAsync(userId, dto, cancellationToken);

            if (!result.Success)
            {
                return BadRequest(new { Error = result.Error });
            }

            return Ok(new { Message = "Password changed successfully. Please login again if required." });
        }


    }

   
}
