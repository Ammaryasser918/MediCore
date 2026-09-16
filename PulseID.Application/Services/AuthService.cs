using PulseID.Application.Common.Exceptions;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.Auth;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PulseID.Application.Services
{
    public class AuthService : IAuthService
    {
        readonly IOtpApiClient _otpClient;
        readonly IIdentityService _identityService;
        readonly IUnitOfWork _unitOfWork;
        readonly IJwtTokenService _jwtTokenService;
        readonly IMemoryCache _cache;

        public AuthService(IOtpApiClient otpApiClient, IIdentityService identityService, IUnitOfWork unitOfWork, IJwtTokenService jwtTokenService, IMemoryCache cache)
        {
            _otpClient = otpApiClient;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
            _jwtTokenService = jwtTokenService;
            _cache = cache;
        }


        // ============== < Patient Registration Section> ===================

        public async Task<OtpApiResult> RequestPatientRegistrationOtpAsync(RegisterPatientDto dto, CancellationToken cancellationToken = default)
        {
            const string genericMessage = "If this email is available an OTP has been sent.";

            if (await _identityService.EmailExistsAsync(dto.Email, cancellationToken))
                return new OtpApiResult { Success = true, Message = genericMessage };

            if (await _unitOfWork.Patients.ExistsByNationalIdAsync(dto.NationalId, cancellationToken))
                return new OtpApiResult { Success = true, Message = genericMessage };

            var registrationResult = await _otpClient.RegisterAsync(dto, cancellationToken);
            if (!registrationResult.Success)
                return registrationResult;

            var result = await _otpClient.RequestOtpAsync(dto.Email, cancellationToken);
            var cacheKey = $"PendingReg_Patient_{dto.Email}";
            _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(10));

            result.Message ??= genericMessage;
            return result;

            
        }


        public async Task<AuthResultDto> VerifyPatientRegistrationAsync(string email, string otp, CancellationToken cancellationToken = default)
        {

            var cacheKey = $"PendingReg_Patient_{email}";
            if (!_cache.TryGetValue(cacheKey, out RegisterPatientDto? dto) || dto is null)
            {
                return new AuthResultDto { Success = false, Error = "Registration session expired or invalid email. Please request a new OTP." };
            }


            try
            {
                var verification = await _otpClient.VerifyOtpAsync(email, otp, cancellationToken);
                if (!verification.Success)
                    return new AuthResultDto { Success = false, Error = verification.Error ?? "Invalid or expired OTP." };



                var (created, userId, error) = await _identityService.CreateUserAsync(dto.Email, dto.Password, "Patient", cancellationToken);

                if (!created || userId == Guid.Empty)
                {
                    await _otpClient.RollbackRegistrationAsync(email, cancellationToken);
                    return new AuthResultDto { Success = false, Error = error ?? "Could not create the account." };
                }

                var patient = new Patient(
                    userId,
                    dto.NationalId,
                    dto.FullName,
                    dto.DateOfBirth,
                    dto.Gender,
                    dto.PhoneNumber
                    );

                await _unitOfWork.Patients.AddAsync(patient, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _cache.Remove(cacheKey);
                var newAccessToken = _jwtTokenService.GenerateToken(userId, "Patient", email);
                var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

                await _identityService.SaveRefreshTokenAsync(userId, newRefreshToken, DateTime.UtcNow.AddDays(10), cancellationToken);

                return new AuthResultDto { Success = true, AccessToken = newAccessToken, RefreshToken = newRefreshToken};
            }
            catch (Exception ex)
            {
                await _otpClient.RollbackRegistrationAsync(email, cancellationToken);
                return new AuthResultDto { Success = false, Error = "An error occurred during registration. Please try again." };
            }
        }


        // ========================== < Doctor Registration Section> =============================

        public async Task<OtpApiResult> RequestDoctorRegistrationOtpAsync(RegisterDoctorDto dto, CancellationToken cancellationToken = default)
        {
            const string genericMessage = "If this email is available an OTP has been sent.";

            if (await _identityService.EmailExistsAsync(dto.Email, cancellationToken))
                return new OtpApiResult { Success = true, Message = genericMessage };

            if (await _unitOfWork.Doctors.ExistsByNationalIdAsync(dto.NationalId, cancellationToken))
                return new OtpApiResult { Success = true, Message = genericMessage };

            var registrationResult = await _otpClient.RegisterAsync(dto, cancellationToken);
            if (!registrationResult.Success)
                return registrationResult;

            var result = await _otpClient.RequestOtpAsync(dto.Email, cancellationToken);
            var cacheKey = $"PendingReg_Doctor_{dto.Email}";
            _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(10));

            result.Message ??= genericMessage;
            return result;
        }

        public async Task<AuthResultDto> VerifyDoctorRegistrationAsync(string email, string otp, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"PendingReg_Doctor_{email}";
            if (!_cache.TryGetValue(cacheKey, out RegisterDoctorDto? dto) || dto is null)
            {
                return new AuthResultDto { Success = false, Error = "Registration session expired or invalid email. Please request a new OTP." };
            }

            try
            {
                var verification = await _otpClient.VerifyOtpAsync(email, otp, cancellationToken);
                if (!verification.Success)
                    return new AuthResultDto { Success = false, Error = verification.Error ?? "Invalid or expired OTP." };

                var (created, userId, error) = await _identityService.CreateUserAsync(dto.Email, dto.Password, "Doctor", cancellationToken);

                if (!created || userId == Guid.Empty)
                {
                    await _otpClient.RollbackRegistrationAsync(email, cancellationToken);
                    return new AuthResultDto { Success = false, Error = error ?? "Could not create the account." };
                }

                var doctor = new Doctor(
                    userId,
                    dto.FullName,
                    dto.SyndicateNumber,
                    dto.SyndicateDocumentPath,
                    dto.NationalId,
                    dto.Specialization,
                    dto.PhoneNumber,
                    dto.DepartmentId,
                    dto.PersonalImagePath
                    );


                await _unitOfWork.Doctors.AddAsync(doctor, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _cache.Remove(cacheKey);
                var newAccessToken = _jwtTokenService.GenerateToken(userId, "Doctor", email);
                var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

                await _identityService.SaveRefreshTokenAsync(userId, newRefreshToken, DateTime.UtcNow.AddDays(10), cancellationToken);

                return new AuthResultDto { Success = true, AccessToken = newAccessToken , RefreshToken = newRefreshToken};

            }
            catch (Exception ex)
            {
                await _otpClient.RollbackRegistrationAsync(email, cancellationToken);
                return new AuthResultDto { Success = false, Error = "An error occurred during registration. Please try again." };
            }
        }




        // ========================== < Pharmacist Registration Section> =============================

        public async Task<OtpApiResult> RequestPharmacistRegistrationOtpAsync(RegisterPharmacistDto dto, CancellationToken cancellationToken = default)
        {
            const string genericMessage = "If this email is available an OTP has been sent.";
            if (await _identityService.EmailExistsAsync(dto.Email, cancellationToken))
                return new OtpApiResult { Success = true, Message = genericMessage };

            if (await _unitOfWork.Pharmacists.ExistsByNationalIdAsync(dto.NationalId, cancellationToken))
                return new OtpApiResult { Success = true, Message = genericMessage };

            var registrationResult = await _otpClient.RegisterAsync(dto, cancellationToken);
            if (!registrationResult.Success)
                return registrationResult;

            var result = await _otpClient.RequestOtpAsync(dto.Email, cancellationToken);

            var cacheKey = $"PendingReg_Pharmacist_{dto.Email}";
            _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(10));

            result.Message ??= genericMessage;
            return result;
        }

        public async Task<AuthResultDto> VerifyPharmacistRegistrationAsync(string email, string otp, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"PendingReg_Pharmacist_{email}";
            if (!_cache.TryGetValue(cacheKey, out RegisterPharmacistDto? dto) || dto is null)
            {
                return new AuthResultDto { Success = false, Error = "Registration session expired or invalid email. Please request a new OTP." };
            }

            try
            {
                var verification = await _otpClient.VerifyOtpAsync(email, otp, cancellationToken);
                if (!verification.Success)
                    return new AuthResultDto { Success = false, Error = verification.Error ?? "Invalid or expired OTP." };

                var (created, userId, error) = await _identityService.CreateUserAsync(dto.Email, dto.Password, "Pharmacist", cancellationToken);

                if (!created || userId == Guid.Empty)
                {
                    await _otpClient.RollbackRegistrationAsync(email, cancellationToken);
                    return new AuthResultDto { Success = false, Error = error ?? "Could not create the account." };
                }

                var pharmacist = new Pharmacist(
                    userId,
                    dto.FullName,
                    dto.SyndicateNumber,
                    dto.SyndicateDocumentPath,
                    dto.NationalId,
                    dto.PhoneNumber,
                    dto.PharmacyId
                    );


                await _unitOfWork.Pharmacists.AddAsync(pharmacist, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _cache.Remove(cacheKey);

                var newAccessToken = _jwtTokenService.GenerateToken(userId, "Pharmacist", email);
                var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

                await _identityService.SaveRefreshTokenAsync(userId, newRefreshToken, DateTime.UtcNow.AddDays(10), cancellationToken);

                return new AuthResultDto { Success = true, AccessToken = newAccessToken, RefreshToken = newRefreshToken };

            }
            catch(Exception ex)
            {
                await _otpClient.RollbackRegistrationAsync(email, cancellationToken);
                return new AuthResultDto { Success = false, Error = "An error occurred during registration. Please try again." };
            }



        }

        // Common

        // ========================== < Login Section> =============================

        public async Task<OtpApiResult> RequestLoginOtpAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            const string genericMessage = "If the credentials are valid, an OTP has been sent.";

            bool userExists = await _identityService.EmailExistsAsync(email, cancellationToken);
            if (!userExists)
                return new OtpApiResult { Success = true, Message = genericMessage };

            if (!await _identityService.CheckPasswordAsync(email, password, cancellationToken))
                return new OtpApiResult { Success = true, Message = genericMessage };

            var result = await _otpClient.RequestOtpAsync(email, cancellationToken);
            result.Message ??= genericMessage;
            return result;

        }

        public async Task<AuthResultDto> VerifyLoginAsync(string email, string otp, CancellationToken cancellationToken = default)
        {
            var verification = await _otpClient.VerifyOtpAsync(email, otp, cancellationToken);
            if (!verification.Success)
                return new AuthResultDto { Success = false, Error = verification.Error ?? "Invalid or expired OTP." };

            var userId = await _identityService.FindUserIdByEmailAsync(email, cancellationToken);
            if (userId == Guid.Empty)
            {
                return new AuthResultDto { Success = false, Error = "Account not found." };
            }

            var role = await _identityService.GetUserRoleAsync(userId, cancellationToken);

            if (string.IsNullOrEmpty(role))
            {
                return new AuthResultDto { Success = false, Error = "User has no assigned role." };
            }

            var newAccessToken = _jwtTokenService.GenerateToken(userId, role, email);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

            await _identityService.SaveRefreshTokenAsync(userId, newRefreshToken, DateTime.UtcNow.AddDays(10), cancellationToken);

            return new AuthResultDto { Success = true, AccessToken = newAccessToken, RefreshToken = newRefreshToken };

        }

        public async Task LogoutAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            await _identityService.RevokeRefreshTokenAsync(userId, cancellationToken);
        }

        public async Task<TokenResponse> RefreshAsync(Guid userId, string refreshToken, CancellationToken cancellationToken = default)
        {
            if (!await _identityService.ValidateRefreshTokenAsync(userId, refreshToken, cancellationToken))
                return null;
            var role = await _identityService.GetUserRoleAsync(userId, cancellationToken);
            var email = await _identityService.GetEmailByUserIdAsync(userId, cancellationToken);

            var newAccessToken = _jwtTokenService.GenerateToken(userId, role, email);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

            await _identityService.RevokeRefreshTokenAsync(userId, cancellationToken);
            await _identityService.SaveRefreshTokenAsync(userId, newRefreshToken, DateTime.UtcNow.AddDays(10), cancellationToken);
            
            return new TokenResponse { AccessToken = newAccessToken, RefreshToken = newRefreshToken };

        }


        // A helper method to get the role of the User
        async Task<string> DetermineRoleAsync(Guid userId, CancellationToken cancellationToken)
        {
            if (await _unitOfWork.Patients.GetByUserIdAsync(userId, cancellationToken) is not null)
                return "Patient";

            if (await _unitOfWork.Doctors.GetByUserIdAsync(userId, cancellationToken) is not null)
                return "Doctor";

            if (await _unitOfWork.Pharmacists.GetByUserIdAsync(userId, cancellationToken) is not null)
                return "Pharmacist";

            return "Unknown";

        }

    }
}
