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
        // Patient Registration
        Task<OtpApiResult> RequestPatientRegistrationOtpAsync(RegisterPatientDto dto, CancellationToken cancellationToken = default);
        Task<AuthResultDto> VerifyPatientRegistrationAsync(string email, string otp, CancellationToken cancellationToken = default);

        // Doctor Registration
        Task<OtpApiResult> RequestDoctorRegistrationOtpAsync(RegisterDoctorDto dto, CancellationToken cancellationToken = default);
        Task<AuthResultDto> VerifyDoctorRegistrationAsync(string email, string otp, CancellationToken cancellationToken = default);

        // Pharmacist Registration

        Task<OtpApiResult> RequestPharmacistRegistrationOtpAsync(RegisterPharmacistDto dto, CancellationToken cancellationToken = default);
        Task<AuthResultDto> VerifyPharmacistRegistrationAsync(string email, string otp, CancellationToken cancellationToken = default);


        // Common
        Task<OtpApiResult> RequestLoginOtpAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<AuthResultDto> VerifyLoginAsync(string email, string otp, CancellationToken cancellationToken = default);
    }
}
