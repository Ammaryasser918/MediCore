using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MediCore.Application.DTOs.OTP;
using MediCore.Application.Interfaces.IServices;

namespace MediCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class OTPController : ControllerBase
    {
        private readonly IOTPService _otpService;
        public OTPController(IOTPService otpService)
        {
            _otpService = otpService;
        }


        [HttpPost("request")]
        [EnableRateLimiting("OTPLimiter")]
        public async Task<IActionResult> RequestOtp([FromBody] RequestOtpDto dto, CancellationToken cancellationToken)
        {
            var result = await _otpService.GenerateAndSendOtpAsync(dto, cancellationToken);
            if (result.Success == false)
                return Ok(new { success = result.Success, message = result.Message });
            return Ok(new { success = result.Success, message = result.Message, Otp = result.Otp });
        }

        [HttpPost("resend")]
        [EnableRateLimiting("OTPLimiter")]
        public async Task<IActionResult> ResendOtp([FromBody] RequestOtpDto dto, CancellationToken cancellationToken)
        {
            var result = await _otpService.ResendOTPAsync(dto, cancellationToken);
            return Ok(new {success = result.Success, message = result.Message, Otp = result.Otp});
        }

        [HttpPost("verify")]
        [EnableRateLimiting("OTPLimiter")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto, CancellationToken cancellationToken)
        {
            var result = await _otpService.VerifyOtpAsync(dto, cancellationToken);
            if (!result.Success)
                return BadRequest(new { success = result.Success, message = result.Message });
            return Ok(new { success = result.Success, message = result.Message, Otp = result.Otp });
        }

    }
}
