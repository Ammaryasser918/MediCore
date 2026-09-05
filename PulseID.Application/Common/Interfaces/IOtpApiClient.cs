using PulseID.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Common.Interfaces
{
    public interface IOtpApiClient
    {
        Task<OtpApiResult> RegisterAsync(RegisterPatientDto dto, CancellationToken cancellationToken = default);
        Task<OtpApiResult> RegisterAsync(RegisterDoctorDto dto, CancellationToken cancellationToken = default);
        Task<OtpApiResult> RequestOtpAsync(string email, CancellationToken cancellationToken = default);
        Task<OtpVerificationResult> VerifyOtpAsync(string email, string Otp, CancellationToken cancellationToken = default);
        Task<bool> RollbackRegistrationAsync(string email, CancellationToken cancellationToken = default);
    }

    public class OtpApiResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public class OtpVerificationResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public string? Token { get; set; }
    }


}
