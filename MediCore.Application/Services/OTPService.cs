using MediCore.Application.Interfaces;
using MediCore.Application.Interfaces.IServices;
using MediCore.Domain.Entities;
using MediCore.Domain.Enums;
using MediCore.Application.DTOs.OTP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediCore.Application.Common.Interfaces;
using MediCore.Application.Common.Exceptions;
using MediCore.Application.DTOs.Doctor;
using System.Numerics;
using MediCore.Application.Common.Templates;

namespace MediCore.Application.Services
{
    public class OTPService : IOTPService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly INotificationSender _notificationSender;
        
        public OTPService(IUnitOfWork unitOfWork, IIdentityService identityService, INotificationSender notificationSender)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _notificationSender = notificationSender;
        }


        public async Task<OtpResultDto> GenerateAndSendOtpAsync(RequestOtpDto dto, CancellationToken cancellationToken = default)
        {

            var emailExists = await _identityService.EmailExistsAsync(dto.Email);

            var haveActiveOtp = await _unitOfWork.Otps.GetLatestActiveAsync(dto.Email, dto.Purpose, cancellationToken);
            if (haveActiveOtp is not null)
                return new OtpResultDto { Success = false, Message = "An OTP has been already sent, Click resend if you didn't receive it." };


            if (dto.Purpose == OtpPurpose.Registration)
            {
                if (emailExists)
                    return new OtpResultDto { Success = true, Message = "If Email Exists, an otp has been sent." };
            }
            else
            {
                if (!emailExists)
                    return new OtpResultDto { Success = true, Message = "If Email Exists, an otp have been sent." };
            }

            var rawCode = System.Security.Cryptography.RandomNumberGenerator.GetInt32(100000, 999999).ToString();

            var codeHash = HashOtpCode(rawCode);

            var otp = new OTP(dto.Email, codeHash, purpose: dto.Purpose, channel: (OtpChannel)dto.Channel, validity: TimeSpan.FromMinutes(5));

            // Notify By Email or whatever
            string emailBody = OtpEmailTemplate.Build(rawCode);

            await _notificationSender.SendAsync(dto.Channel, recipient: dto.Email, subject: "MediCore - Verification Code", message: emailBody, cancellationToken: cancellationToken);

            await _unitOfWork.Otps.AddAsync(otp, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new OtpResultDto
            {
                Success = true,
                Message = "If Email Exists, an otp have been sent.",
                // OTP is temporary while development only
                Otp = rawCode
            };


        }

        public async Task<OtpResultDto> ResendOTPAsync(RequestOtpDto dto, CancellationToken cancellationToken = default)
        {
            var currentOtp = await _unitOfWork.Otps.GetLatestActiveAsync(dto.Email, dto.Purpose, cancellationToken);

            if (currentOtp is not null)
            {
                var timeSinceCreation = DateTime.UtcNow - currentOtp.CreatedAt;
                if (timeSinceCreation < TimeSpan.FromSeconds(60))
                {
                    var remainingSeconds = 60 - (int)timeSinceCreation.TotalSeconds;
                    return new OtpResultDto { Success = false, Message = $"Please wait {remainingSeconds} seconds before requestinga new OTP" };
                }
                currentOtp.SoftDelete();
            }

            return await GenerateAndSendOtpAsync(dto, cancellationToken);
            

        }

        public async Task<OtpResultDto> VerifyOtpAsync(VerifyOtpDto dto, CancellationToken cancellationToken = default)
        {
            var otpRecord = await _unitOfWork.Otps.GetLatestActiveAsync(dto.Email, dto.Purpose, cancellationToken);
            if (otpRecord is null || !otpRecord.CanBeValidated())
                return new OtpResultDto { Success = false, Message = "Invalid or Expired OTP." };

            var inputHash = HashOtpCode(dto.Otp);
            bool isMatch = otpRecord.CodeHash == inputHash;

            otpRecord.RegisterAttempt(isMatch);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!isMatch)
                return new OtpResultDto { Success = false, Message = "Invalid Or Expired OTP." };

            return new OtpResultDto { Success = true, Message = "OTP verified successfully" };

        }

        private string HashOtpCode(string code)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(code);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

    }
}
