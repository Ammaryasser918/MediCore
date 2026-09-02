using Microsoft.AspNetCore.Mvc;
using PulseID.Otp;
using PulseID.Application.Common.Interfaces;

namespace PulseID.API
{
    /// <summary>
    /// مثال يوضّح إزاي الـ Controller بتاعك يستخدم OtpApiClient.
    /// انسخ المنطق ده لأي Controller عندك محتاج تحقق بـ OTP.
    /// </summary>
    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase
    {
        private readonly IOtpApiClient _otpClient;

        public AccountController(IOtpApiClient otpClient)
        {
            _otpClient = otpClient;
        }

        // POST /account/send-code
        [HttpPost("send-code")]
        public async Task<IActionResult> SendCode([FromBody] string email)
        {
            var result = await _otpClient.RequestOtpAsync(email);
            return Ok(result); // بيرجع رسالة عامة زي "If this account exists..."
        }

        // POST /account/confirm-code
        [HttpPost("confirm-code")]
        public async Task<IActionResult> ConfirmCode([FromBody] VerifyOtpRequest request)
        {
            var result = await _otpClient.VerifyOtpAsync(request.Email, request.Otp);

            if (!result.Success)
            {
                // Reject
                return BadRequest(new { message = result.Error });
            }

            // Success -- result.Token فيه الـ JWT الصادر من الـ Python server
            // ممكن تخزنه في Cookie أو تستخدمه زي ما يريحك في نظام الـ Auth بتاعك
            return Ok(new { message = "تم التحقق بنجاح", token = result.Token });
        }
    }
}
