using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.Auth;
using PulseID.Application.DTOs.OTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.IServices
{
    public interface IAuthService
    {
        // Patient Registration
        Task<OtpResultDto> RequestPatientRegistrationOtpAsync(RegisterPatientDto dto, CancellationToken cancellationToken = default);
        Task<AuthResultDto> VerifyPatientRegistrationAsync(string email, string otp, CancellationToken cancellationToken = default);

        // Doctor Registration
        Task<OtpResultDto> RequestDoctorRegistrationOtpAsync(RegisterDoctorDto dto, CancellationToken cancellationToken = default);
        Task<AuthResultDto> VerifyDoctorRegistrationAsync(string email, string otp, CancellationToken cancellationToken = default);

        // Pharmacist Registration

        Task<OtpResultDto> RequestPharmacistRegistrationOtpAsync(RegisterPharmacistDto dto, CancellationToken cancellationToken = default);
        Task<AuthResultDto> VerifyPharmacistRegistrationAsync(string email, string otp, CancellationToken cancellationToken = default);
        
        // Lab Technician Registration

        Task<OtpResultDto> RequestLabTechnicianRegistrationOtpAsync(RegisterLabTechnicianDto dto, CancellationToken cancellationToken = default);
        Task<AuthResultDto> VerifyLabTechnicianRegistrationAsync(string email, string otp, CancellationToken cancellationToken = default);


        // Common
        Task<OtpResultDto> RequestLoginOtpAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<AuthResultDto> VerifyLoginAsync(string email, string otp, CancellationToken cancellationToken = default);
        Task LogoutAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<TokenResponse> RefreshAsync(Guid userId, string refreshToken,  CancellationToken cancellationToken = default);
        Task<AuthResultDto> ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken cancellationToken = default);
    }
}
