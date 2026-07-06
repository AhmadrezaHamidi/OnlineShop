/// <summary>Fake پیاده‌سازی IOtpRepository با کنترل کامل بر OTP برای سناریوهای مختلف</summary>
using Identity.Domain.Entities;
using Identity.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Tests.Fakes.Identity;

public class FakeOtpRepository : IOtpRepository
{
    private readonly Dictionary<string, OtpRequest> _latest = new();
    private long _nextId = 1;

    public OtpRequest? Added   { get; private set; }
    public OtpRequest? Updated { get; private set; }

    public Task<OtpRequest?> GetLatestByPhoneAsync(string phoneNumber, CancellationToken token = default)
        => Task.FromResult(_latest.GetValueOrDefault(phoneNumber));

    public Task AddAsync(OtpRequest otp, CancellationToken token = default)
    {
        Added = otp;
        _latest[otp.PhoneNumber] = otp;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(OtpRequest otp, CancellationToken token = default)
    {
        Updated = otp;
        _latest[otp.PhoneNumber] = otp;
        return Task.CompletedTask;
    }

    public Task<long> GetNextIdAsync() => Task.FromResult(_nextId++);

    // ─── Helper factories برای سناریوهای مختلف ─────────────────────────────

    /// <summary>OTP معتبر با کد مشخص — سناریوی موفق</summary>
    public OtpRequest SeedValidOtp(string phone, string code = "123456")
    {
        var otp = new OtpRequest(1, phone, code, expiryMinutes: 5);
        _latest[phone] = otp;
        return otp;
    }

    /// <summary>OTP منقضی‌شده — سناریوی Expired</summary>
    public OtpRequest SeedExpiredOtp(string phone, string code = "654321")
    {
        var otp = new OtpRequest(2, phone, code, expiryMinutes: -1); // فوری منقضی
        _latest[phone] = otp;
        return otp;
    }

    /// <summary>OTP قبلاً استفاده‌شده — سناریوی Replay Attack</summary>
    public OtpRequest SeedUsedOtp(string phone, string code = "111111")
    {
        var otp = new OtpRequest(3, phone, code, expiryMinutes: 5);
        otp.Verify(code); // استفاده می‌کنیم تا IsUsed = true شود
        _latest[phone] = otp;
        return otp;
    }
}
