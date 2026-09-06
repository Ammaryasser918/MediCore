using System.Diagnostics;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.Auth;

namespace PulseID.Otp
{
    

    public class OtpApiClient : IOtpApiClient
    {
        private readonly HttpClient _http;

        public OtpApiClient(HttpClient http, IOptions<OtpApiOptions> options)
        {
            _http = http;
            _http.BaseAddress = new Uri(options.Value.BaseUrl);
            _http.Timeout = TimeSpan.FromSeconds(options.Value.TimeoutSeconds);
        }

        public async Task<OtpApiResult> RegisterAsync(RegisterPatientDto dto, CancellationToken cancellationToken = default)
        {
            var body = new RegisterRequest { Email = dto.Email, Password = dto.Password, Phone = dto.PhoneNumber, Name = dto.FullName, NationalId = dto.NationalId, Gender = dto.Gender };
            var response = await _http.PostAsJsonAsync("/api/auth/register", body, cancellationToken);
            var wire = await ReadOrDefaultAsync<GenericMessageResponse>(response, cancellationToken);
            return new OtpApiResult { Success = response.IsSuccessStatusCode, Message = wire.Message ?? wire.Error };
        }

        public async Task<OtpApiResult> RegisterAsync(RegisterDoctorDto dto, CancellationToken cancellationToken = default)
        {
            var body = new RegisterRequest { Email = dto.Email, Password = dto.Password, Phone = dto.PhoneNumber, Name = dto.FullName, NationalId = dto.NationalId };
            var response = await _http.PostAsJsonAsync("/api/auth/register", body, cancellationToken);
            var wire = await ReadOrDefaultAsync<GenericMessageResponse>(response, cancellationToken);
            return new OtpApiResult { Success = response.IsSuccessStatusCode, Message = wire.Message ?? wire.Error };
        }
        
        public async Task<OtpApiResult> RegisterAsync(RegisterPharmacistDto dto, CancellationToken cancellationToken = default)
        {
            var body = new RegisterRequest { Email = dto.Email, Password = dto.Password, Phone = dto.PhoneNumber, Name = dto.FullName, NationalId = dto.NationalId };
            var response = await _http.PostAsJsonAsync("/api/auth/register", body, cancellationToken);
            var wire = await ReadOrDefaultAsync<GenericMessageResponse>(response, cancellationToken);
            return new OtpApiResult { Success = response.IsSuccessStatusCode, Message = wire.Message ?? wire.Error };
        }

        public async Task<OtpApiResult> RequestOtpAsync(string email, CancellationToken cancellationToken = default)
        {
            var body = new RequestOtpRequest { Email = email };
            var response = await _http.PostAsJsonAsync("/api/auth/request-otp", body, cancellationToken);
            var wire = await ReadOrDefaultAsync<GenericMessageResponse>(response, cancellationToken);
            return new OtpApiResult { Success = response.IsSuccessStatusCode, Message = wire.Message ?? wire.Error };
        }

        public async Task<OtpVerificationResult> VerifyOtpAsync(string email, string otp, CancellationToken ct = default)
        {
            var body = new VerifyOtpRequest { Email = email, Otp = otp };
            var response = await _http.PostAsJsonAsync("/api/auth/verify-otp", body, ct);

            // الـ Python API بيرجع 400 في حالة الرفض، فمنعملش EnsureSuccessStatusCode هنا
            var result = await response.Content.ReadFromJsonAsync<VerifyOtpResponse>(cancellationToken: ct);
            if (result is null)
                return new OtpVerificationResult { Success = false, Error = "Empty response from OTP server." };

            if (!response.IsSuccessStatusCode || !string.IsNullOrEmpty(result.Error))
                return new OtpVerificationResult { Success = false, Error = result.Error ?? "OTP verification failed." };

            return new OtpVerificationResult { Success = true, Error = result.Error, Token = result.Token };
        }

        public async Task<bool> RollbackRegistrationAsync(string email, CancellationToken cancellationToken = default)
        {
            var response = await _http.DeleteAsync($"/api/auth/user/{email}", cancellationToken);
            return response.IsSuccessStatusCode;
        }

        private static async Task<T> ReadOrDefaultAsync<T>(HttpResponseMessage response, CancellationToken ct) where T : new()
        {
            var result = await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
            return result ?? new T();
        }
    }
}
