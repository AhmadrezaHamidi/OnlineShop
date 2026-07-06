/// <summary>
/// سناریوهای اعتبارسنجی OTP — همه حالت‌های خطا
/// ─────────────────────────────────────────────────────────────────────────────
/// سناریو ۱: OTP اشتباه        → InvalidOtpException
/// سناریو ۲: OTP منقضی‌شده    → InvalidOtpException
/// سناریو ۳: OTP تکراری        → InvalidOtpException (Replay Attack Prevention)
/// سناریو ۴: OTP درخواست نشده  → OtpNotRequestedException
/// سناریو ۵: SMS ارسال نشد     → false برگردد
/// </summary>
using Ahmad.OnlineShop.Application.Tests.Fakes;
using Ahmad.OnlineShop.Application.Tests.Fakes.Identity;
using Identity.Application.Commands;
using Identity.Application.Handlers;
using IdentityUser = Identity.Domain.Aggregates.User;
using Identity.Domain.Exceptions;

namespace Ahmad.OnlineShop.Application.Tests.Identity;

public class OtpValidationScenarioTests
{
    private const string Phone     = "09121111111";
    private const string ValidCode = "123456";
    private const string WrongCode = "999999";

    private readonly FakeIdentityUserRepository _userRepo    = new();
    private readonly FakeOtpRepository          _otpRepo     = new();
    private readonly FakeRefreshTokenRepository _refreshRepo = new();
    private readonly FakeIdentityRoleRepository _roleRepo    = new();
    private readonly FakeSmsService             _sms         = new();
    private readonly FakeJwtService             _jwt         = new();
    private readonly IdentityHandlers           _sut;
    private readonly CancellationToken          _ct = CancellationToken.None;

    public OtpValidationScenarioTests()
    {
        _sut = new IdentityHandlers(_userRepo, _refreshRepo, _roleRepo, _otpRepo, _sms, _jwt, FakeIdentityDb.Create());
    }

    // ── سناریو ۱: کد اشتباه ────────────────────────────────────────────────

    /// <summary>ورود با کد اشتباه باید InvalidOtpException بدهد</summary>
    [Fact]
    public async Task Scenario1_WrongCode_Should_Throw_InvalidOtpException()
    {
        _otpRepo.SeedValidOtp(Phone, ValidCode);

        await Assert.ThrowsAsync<InvalidOtpException>(
            () => _sut.Handle(new VerifyOtpCommand(Phone, WrongCode), _ct));
    }

    /// <summary>بعد از کد اشتباه، کاربر نباید ساخته شود</summary>
    [Fact]
    public async Task Scenario1_WrongCode_Should_NOT_CreateUser()
    {
        _otpRepo.SeedValidOtp(Phone, ValidCode);

        try { await _sut.Handle(new VerifyOtpCommand(Phone, WrongCode), _ct); }
        catch (InvalidOtpException) { }

        Assert.Null(_userRepo.Added);
    }

    // ── سناریو ۲: OTP منقضی ────────────────────────────────────────────────

    /// <summary>OTP منقضی‌شده باید InvalidOtpException بدهد</summary>
    [Fact]
    public async Task Scenario2_ExpiredOtp_Should_Throw_InvalidOtpException()
    {
        _otpRepo.SeedExpiredOtp(Phone, ValidCode);

        await Assert.ThrowsAsync<InvalidOtpException>(
            () => _sut.Handle(new VerifyOtpCommand(Phone, ValidCode), _ct));
    }

    /// <summary>OTP منقضی باید IsExpired = true باشد</summary>
    [Fact]
    public async Task Scenario2_ExpiredOtp_IsExpired_Should_Be_True()
    {
        var otp = _otpRepo.SeedExpiredOtp(Phone, ValidCode);

        Assert.True(otp.IsExpired);
    }

    // ── سناریو ۳: Replay Attack ─────────────────────────────────────────────

    /// <summary>استفاده دوباره از OTP باید InvalidOtpException بدهد</summary>
    [Fact]
    public async Task Scenario3_ReplayAttack_UsedOtp_Should_Throw_InvalidOtpException()
    {
        _otpRepo.SeedUsedOtp(Phone, ValidCode);

        await Assert.ThrowsAsync<InvalidOtpException>(
            () => _sut.Handle(new VerifyOtpCommand(Phone, ValidCode), _ct));
    }

    /// <summary>OTP استفاده‌شده IsUsed = true است</summary>
    [Fact]
    public async Task Scenario3_AfterVerify_OtpShouldBeMarkedUsed_And_SecondVerify_Should_Fail()
    {
        // اولین verify موفق
        _otpRepo.SeedValidOtp(Phone, ValidCode);
        await _sut.Handle(new VerifyOtpCommand(Phone, ValidCode), _ct);

        // دومین verify با همان OTP
        await Assert.ThrowsAsync<InvalidOtpException>(
            () => _sut.Handle(new VerifyOtpCommand(Phone, ValidCode), _ct));
    }

    // ── سناریو ۴: OTP درخواست نشده ─────────────────────────────────────────

    /// <summary>تأیید بدون RequestOtp باید OtpNotRequestedException بدهد</summary>
    [Fact]
    public async Task Scenario4_NoOtpRequested_Should_Throw_OtpNotRequestedException()
    {
        await Assert.ThrowsAsync<OtpNotRequestedException>(
            () => _sut.Handle(new VerifyOtpCommand(Phone, ValidCode), _ct));
    }

    // ── سناریو ۵: SMS ارسال نشد ─────────────────────────────────────────────

    /// <summary>شکست در ارسال SMS باید false برگرداند</summary>
    [Fact]
    public async Task Scenario5_SmsFails_Should_Return_False()
    {
        _sms.ShouldFail = true;

        var result = await _sut.Handle(new RequestOtpCommand(Phone), _ct);

        Assert.False(result);
    }

    /// <summary>حتی اگر SMS نرفت، OTP در دیتابیس ذخیره شده</summary>
    [Fact]
    public async Task Scenario5_SmsFails_OtpStill_Saved_To_Repository()
    {
        _sms.ShouldFail = true;

        await _sut.Handle(new RequestOtpCommand(Phone), _ct);

        Assert.NotNull(_otpRepo.Added);
    }

    // ── تست اضافی: فرمت کد OTP ──────────────────────────────────────────────

    /// <summary>کد OTP باید دقیقاً ۶ رقم باشد</summary>
    [Fact]
    public async Task OtpCode_Generated_Should_Be_6_Digits()
    {
        await _sut.Handle(new RequestOtpCommand(Phone), _ct);

        var code = _sms.LastCode!;
        Assert.Equal(6,       code.Length);
        Assert.True(int.TryParse(code, out var num));
        Assert.True(num >= 100_000 && num <= 999_999);
    }
}
