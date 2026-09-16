namespace PulseID.Otp
{
    /// <summary>
    /// إعدادات الاتصال بسيرفر الـ OTP (Python/Flask).
    /// بتتقرأ من appsettings.json تحت section اسمها "OtpApi".
    /// </summary>
    public class OtpApiOptions
    {
        public string BaseUrl { get; set; } = "http://localhost:5000";
        public int TimeoutSeconds { get; set; } = 10;
    }
}
