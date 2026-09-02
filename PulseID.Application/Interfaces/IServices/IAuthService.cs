using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<OtpApiResult> RequestPatientRegistrationOtpAsync(RegisterPatientDto dto, CancellationToken cancellationToken = default);
        Task<AuthResultDto> VerifyPatientRegistrationAsync(string email, string otp, CancellationToken cancellationToken = default);
        Task<OtpApiResult> RequestLoginOtpAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<AuthResultDto> VerifyLoginAsync(string email, string otp, CancellationToken cancellationToken = default);
    }
}
