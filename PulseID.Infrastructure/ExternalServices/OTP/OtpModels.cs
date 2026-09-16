using PulseID.Domain.Enums;
using System.Text.Json.Serialization;

namespace PulseID.Otp
{
    // ---------- Requests ----------
    // ملحوظة مهمة: الـ Python API بيتوقع أسماء الحقول بحروف صغيرة (email, otp, ...).
    // JsonPropertyName هنا إجباري عشان System.Text.Json الافتراضي في C# بيبعت
    // الأسماء PascalCase (زي الـ property نفسها)، وده هيخلي الـ Python يرفض الطلب بصمت.
    public class RequestOtpRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
    }
    
    public class VerifyOtpRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("otp")]
        public string Otp { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; } // بيستخدم في تحية قالب رسالة الإيميل ("Hi <name>,")

        [JsonPropertyName("nationalId")]
        public string NationalId { get; set; } = string.Empty;

        [JsonPropertyName("gender")]
        public Gender? Gender { get; set; }
    }

    // ---------- Responses ----------
    // هنا مش لازم JsonPropertyName لأن System.Text.Json بيقدر يطابق
    // "message"/"error" مع Message/Error case-insensitive عند القراءة تلقائيًا،
    // لكن بنحطها برضو للوضوح والأمان.
    public class GenericMessageResponse
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }

    public class VerifyOtpResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; } // JWT لو Success = true
    }
}

