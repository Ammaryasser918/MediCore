using PulseID.Application.DTOs.OTP;
using PulseID.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.IServices
{
    public interface IOTPService
    {
        Task<OtpResultDto> GenerateAndSendOtpAsync(RequestOtpDto dto, CancellationToken cancellationToken = default);
        Task<OtpResultDto> ResendOTPAsync(RequestOtpDto dto, CancellationToken cancellationToken = default);
        Task<OtpResultDto> VerifyOtpAsync(VerifyOtpDto dto, CancellationToken cancellationToken = default);

    }
}
