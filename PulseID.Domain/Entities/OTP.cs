using PulseID.Domain.Common;
using PulseID.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Domain.Entities
{
    public class OTP : BaseEntity
    {

        public const int MaxAttempts = 5;

        public Guid UserId { get; private set; } = default!;
        public string CodeHash { get; private set; } = default!;
        public OtpPurpose Purpose { get; private set; }
        public OtpChannel Channel { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsUsed { get; private set; }
        public int AttemptCount { get; private set; }


        OTP() { }

        public OTP(Guid userId, string codeHash, OtpPurpose purpose, OtpChannel channel, TimeSpan validity)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User Id is required", nameof(userId));
            if (string.IsNullOrWhiteSpace(codeHash))
                throw new ArgumentException("Code hash is required", nameof(codeHash));
            UserId = userId;
            CodeHash = codeHash;
            Purpose = purpose;
            Channel = channel;
            ExpiresAt = DateTime.UtcNow.Add(validity);
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
