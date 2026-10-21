using MediCore.Application.DTOs.OTP;
using MediCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Interfaces.IServices
{
    public interface IOTPService
    {
        Task<OtpResultDto> GenerateAndSendOtpAsync(RequestOtpDto dto, CancellationToken cancellationToken = default);
        Task<OtpResultDto> ResendOTPAsync(RequestOtpDto dto, CancellationToken cancellationToken = default);
        Task<OtpResultDto> VerifyOtpAsync(VerifyOtpDto dto, CancellationToken cancellationToken = default);

    }
}
