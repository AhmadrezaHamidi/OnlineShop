// این فایل فقط یک alias است — ISmsService از AhmadBase.External.Service می‌آید
// global using ISmsService = AhmadBase.External.Service.ISmsService;
// فعلاً تا NuGet نسخه جدید publish نشود، interface محلی نگه داشته می‌شود

namespace Identity.Application.Services;

/// <summary>در نسخه بعدی NuGet (5.1.0) از AhmadBase.External.Service.ISmsService استفاده خواهد شد.</summary>
public interface ISmsService
{
    Task<bool> SendOtpAsync(string phoneNumber, string code, CancellationToken token = default);
    Task<bool> SendBulkAsync(IReadOnlyList<string> phones, string message, CancellationToken token = default);
}
