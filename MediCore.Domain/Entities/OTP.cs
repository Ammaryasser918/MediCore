using MediCore.Domain.Common;
using MediCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Domain.Entities
{
    public class OTP : BaseEntity
    {

        public const int MaxAttempts = 5;

        public Guid? UserId { get; private set; }
        public string Email { get; private set; } = default!;
        public string CodeHash { get; private set; } = default!;
        public OtpPurpose Purpose { get; private set; }
        public OtpChannel Channel { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsUsed { get; private set; }
        public int AttemptCount { get; private set; }


        OTP() { }

        public OTP(string email, string codeHash, OtpPurpose purpose, OtpChannel channel, TimeSpan validity, Guid? userId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required", nameof(email));
            if (string.IsNullOrWhiteSpace(codeHash))
                throw new ArgumentException("Code hash is required", nameof(codeHash));
            Email = email;
            CodeHash = codeHash;
            Purpose = purpose;
            Channel = channel;
            ExpiresAt = DateTime.UtcNow.Add(validity);
            UserId = userId;
            AttemptCount = 0;
        }

        public bool IsExpired() => DateTime.UtcNow > ExpiresAt;

        public bool HasExceededMaxAttempts() => AttemptCount >= MaxAttempts;

        public bool CanBeValidated() => !IsUsed && !IsExpired() && !HasExceededMaxAttempts();


        public void RegisterAttempt(bool isMatch)
        {
            AttemptCount++;
            MarkAsUpdated();
            if (isMatch) IsUsed = true;
        }




    }
}
