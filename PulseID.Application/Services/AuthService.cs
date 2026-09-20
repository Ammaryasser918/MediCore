using PulseID.Application.Common.Exceptions;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.Auth;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Entities;
using PulseID.Domain.Enums;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using PulseID.Application.DTOs.OTP;
using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;

namespace PulseID.Application.Services
{
    public class AuthService : IAuthService
    {
        readonly IOTPService _otpService;
        readonly IIdentityService _identityService;
        readonly IUnitOfWork _unitOfWork;
        readonly IJwtTokenService _jwtTokenService;
        readonly IDataProtector _protector;

        public AuthService(IOTPService otpService, IIdentityService identityService, IUnitOfWork unitOfWork, IJwtTokenService jwtTokenService, IDataProtectionProvider provider)
        {
            _otpService = otpService;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
            _jwtTokenService = jwtTokenService;
            _protector = provider.CreateProtector("PulseID.PatientRegistration.v1");
        }


        // ============== < Patient Registration Section> ===================

        public async Task<OtpResultDto> RequestPatientRegistrationOtpAsync(RegisterPatientDto dto, CancellationToken cancellationToken = default)
        {
            const string genericMessage = "If this email is available an OTP has been sent.";

            if (await _identityService.EmailExistsAsync(dto.Email, cancellationToken))
                return new OtpResultDto { Success = true, Message = genericMessage };

            if (await _unitOfWork.Patients.ExistsByNationalIdAsync(dto.NationalId, cancellationToken))
                return new OtpResultDto { Success = true, Message = genericMessage };

            var requestDto = new RequestOtpDto
            {
                Email = dto.Email,
                Purpose = OtpPurpose.Registration,
                Channel = OtpChannel.Email
            };
        
            var registrationResult = await _otpService.GenerateAndSendOtpAsync(requestDto, cancellationToken);
            if (!registrationResult.Success)
                return registrationResult;

            var jsonDto = JsonSerializer.Serialize(dto);
            var registerationToken = _protector.Protect(jsonDto);
            registrationResult.RegistrationToken = registerationToken; 

            return registrationResult;

            
        }


        public async Task<AuthResultDto> VerifyPatientRegistrationAsync(string registrationToken, string otp, CancellationToken cancellationToken = default)
        {

            RegisterPatientDto? dto;
            try
            {
                var jsonDto = _protector.Unprotect(registrationToken);
                dto = JsonSerializer.Deserialize<RegisterPatientDto>(jsonDto);

                if (dto is null)
                    return new AuthResultDto { Success = false, Error = "Invalid Registration Payload." };
            }
            catch (Exception)
            {
                return new AuthResultDto { Success = false, Error = "Registration session expired or token is invalid." };
            }


            var verifyOtpDto = new VerifyOtpDto
            {
                Email = dto.Email,
                Purpose = OtpPurpose.Registration,
                Otp = otp
            };
            var verification = await _otpService.VerifyOtpAsync(verifyOtpDto, cancellationToken);
            if (!verification.Success)
                return new AuthResultDto { Success = false, Error = verification.Message ?? "Invalid or expired OTP." };



            var (created, userId, error) = await _identityService.CreateUserAsync(dto.Email, dto.Password, "Patient", cancellationToken);

            if (!created || userId == Guid.Empty)
            {
                return new AuthResultDto { Success = false, Error = error ?? "Could not create the account." };
            }

            try
            {
                

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
                var newAccessToken = _jwtTokenService.GenerateToken(userId, "Patient", dto.Email);
                var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

                await _identityService.SaveRefreshTokenAsync(userId, newRefreshToken, DateTime.UtcNow.AddDays(10), cancellationToken);

                return new AuthResultDto { Success = true, AccessToken = newAccessToken, RefreshToken = newRefreshToken};
            }
            catch (Exception)
            {
                await _identityService.DeleteUserAsync(userId, cancellationToken);
                return new AuthResultDto { Success = false, Error = "An error occurred during registration. Please try again." };
            }
        }


        // ========================== < Doctor Registration Section> =============================

        public async Task<OtpResultDto> RequestDoctorRegistrationOtpAsync(RegisterDoctorDto dto, CancellationToken cancellationToken = default)
        {
            const string genericMessage = "If this email is available an OTP has been sent.";

            if (await _identityService.EmailExistsAsync(dto.Email, cancellationToken))
                return new OtpResultDto { Success = true, Message = genericMessage };

            if (await _unitOfWork.Doctors.ExistsByNationalIdAsync(dto.NationalId, cancellationToken))
                return new OtpResultDto { Success = true, Message = genericMessage };

            var requestDto = new RequestOtpDto
            {
                Email = dto.Email,
                Purpose = OtpPurpose.Registration,
                Channel = OtpChannel.Email
            };

            var registrationResult = await _otpService.GenerateAndSendOtpAsync(requestDto, cancellationToken);
            if (!registrationResult.Success)
                return registrationResult;

            var jsonDto = JsonSerializer.Serialize(dto);
            var registerationToken = _protector.Protect(jsonDto);
            registrationResult.RegistrationToken = registerationToken;

            return registrationResult;
        }

        public async Task<AuthResultDto> VerifyDoctorRegistrationAsync(string registrationToken, string otp, CancellationToken cancellationToken = default)
        {
            RegisterDoctorDto? dto;
            try
            {
                var jsonDto = _protector.Unprotect(registrationToken);
                dto = JsonSerializer.Deserialize<RegisterDoctorDto>(jsonDto);
                if (dto is null)
                    return new AuthResultDto { Success = false, Error = "Invalid registation token." };
            }
            catch (Exception)
            {
                return new AuthResultDto { Success = false, Error = "Registration session expired or token is invalid" };
            }


            var verifyDto = new VerifyOtpDto
            {
                Email = dto.Email,
                Purpose = OtpPurpose.Registration,
                Otp = otp
            };
            var verification = await _otpService.VerifyOtpAsync(verifyDto, cancellationToken);
            if (!verification.Success)
                return new AuthResultDto { Success = false, Error = verification.Message ?? "Invalid or expired OTP." };

            var (created, userId, error) = await _identityService.CreateUserAsync(dto.Email, dto.Password, "Doctor", cancellationToken);

            if (!created || userId == Guid.Empty)
            {
                return new AuthResultDto { Success = false, Error = error ?? "Could not create the account." };
            }


            try
            {
                

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
                
                var newAccessToken = _jwtTokenService.GenerateToken(userId, "Doctor", dto.Email);
                var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

                await _identityService.SaveRefreshTokenAsync(userId, newRefreshToken, DateTime.UtcNow.AddDays(10), cancellationToken);

                return new AuthResultDto { Success = true, AccessToken = newAccessToken , RefreshToken = newRefreshToken};

            }
            catch (Exception)
            {
                await _identityService.DeleteUserAsync(userId, cancellationToken);
                return new AuthResultDto { Success = false, Error = "An error occurred during registration. Please try again." };
            }
        }




        // ========================== < Pharmacist Registration Section> =============================

        public async Task<OtpResultDto> RequestPharmacistRegistrationOtpAsync(RegisterPharmacistDto dto, CancellationToken cancellationToken = default)
        {
            const string genericMessage = "If this email is available an OTP has been sent.";
            if (await _identityService.EmailExistsAsync(dto.Email, cancellationToken))
                return new OtpResultDto { Success = true, Message = genericMessage };

            if (await _unitOfWork.Pharmacists.ExistsByNationalIdAsync(dto.NationalId, cancellationToken))
                return new OtpResultDto { Success = true, Message = genericMessage };

            var requestDto = new RequestOtpDto
            {
                Email = dto.Email,
                Purpose = OtpPurpose.Registration,
                Channel = OtpChannel.Email
            };            
            
            var registrationResult = await _otpService.GenerateAndSendOtpAsync(requestDto, cancellationToken);
            if (!registrationResult.Success)
                return registrationResult;

            var jsonDto = JsonSerializer.Serialize(dto);
            var registrationToken = _protector.Protect(jsonDto);
            registrationResult.RegistrationToken = registrationToken;

            return registrationResult;
        }

        public async Task<AuthResultDto> VerifyPharmacistRegistrationAsync(string registrationToken, string otp, CancellationToken cancellationToken = default)
        {
            RegisterPharmacistDto? dto;
            try
            {
                var jsonDto = _protector.Unprotect(registrationToken);
                dto = JsonSerializer.Deserialize<RegisterPharmacistDto>(jsonDto);
                if (dto is null)
                    return new AuthResultDto { Success = false, Error = "Invalid registration Payload." };
            }
            catch (Exception)
            {
                return new AuthResultDto { Success = false, Error = "Registration session expired or token is invalid." };
            }

            var verifyDto = new VerifyOtpDto
            {
                Email = dto.Email,
                Purpose = OtpPurpose.Registration,
                Otp = otp
            };
            var verification = await _otpService.VerifyOtpAsync(verifyDto, cancellationToken);
            if (!verification.Success)
                return new AuthResultDto { Success = false, Error = verification.Message ?? "Invalid or expired OTP." };

            var (created, userId, error) = await _identityService.CreateUserAsync(dto.Email, dto.Password, "Pharmacist", cancellationToken);

            if (!created || userId == Guid.Empty)
            {
                return new AuthResultDto { Success = false, Error = error ?? "Could not create the account." };
            }

            try
            {
                

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


                var newAccessToken = _jwtTokenService.GenerateToken(userId, "Pharmacist", dto.Email);
                var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

                await _identityService.SaveRefreshTokenAsync(userId, newRefreshToken, DateTime.UtcNow.AddDays(10), cancellationToken);

                return new AuthResultDto { Success = true, AccessToken = newAccessToken, RefreshToken = newRefreshToken };

            }
            catch(Exception)
            {
                await _identityService.DeleteUserAsync(userId, cancellationToken);
                return new AuthResultDto { Success = false, Error = "An error occurred during registration. Please try again." };
            }



        }

        // Common

        // ========================== < Login Section> =============================

        public async Task<OtpResultDto> RequestLoginOtpAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            const string genericMessage = "If the credentials are valid, an OTP has been sent.";

            bool userExists = await _identityService.EmailExistsAsync(email, cancellationToken);
            if (!userExists)
                return new OtpResultDto { Success = true, Message = genericMessage };

            if (!await _identityService.CheckPasswordAsync(email, password, cancellationToken))
                return new OtpResultDto { Success = true, Message = genericMessage };

            var requstDto = new RequestOtpDto
            {
                Email = email,
                Purpose = OtpPurpose.Login,
                Channel = OtpChannel.Email
            };

            var result = await _otpService.GenerateAndSendOtpAsync(requstDto, cancellationToken);
            result.Message = genericMessage;
            return result;

           

        }

        public async Task<AuthResultDto> VerifyLoginAsync(string email, string otp, CancellationToken cancellationToken = default)
        {
            var verifyDto = new VerifyOtpDto
            {
                Email = email,
                Purpose = OtpPurpose.Login,
                Otp = otp
            };

            var verification = await _otpService.VerifyOtpAsync(verifyDto, cancellationToken);
            if (!verification.Success)
                return new AuthResultDto { Success = false, Error = verification.Message ?? "Invalid or expired OTP." };

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
