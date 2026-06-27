/// <summary>
/// سناریو: ورود مشتری جدید (اولین بار)
/// ─────────────────────────────────────────────────────────────────────────────
/// گام ۱: RequestOtp(phone) → SMS ارسال شود
/// گام ۲: VerifyOtp(phone, code) → کاربر جدید ساخته و JWT صادر شود
///
/// انتظار: کاربر خودکار در سیستم ثبت شود، UserType = Customer
/// </summary>
using Ahmad.OnlineShop.Application.Tests.Fakes.Identity;
using Identity.Application.Commands;
using Identity.Application.Handlers;
using IdentityUser = Identity.Domain.Aggregates.User;
using Identity.Domain.Enums;

namespace Ahmad.OnlineShop.Application.Tests.Identity;

public class NewCustomerLoginScenarioTests
{
    private const string Phone = "09121111111";
    private const string Code  = "123456";

    private readonly FakeIdentityUserRepository _userRepo     = new();
    private readonly FakeOtpRepository          _otpRepo      = new();
    private readonly FakeRefreshTokenRepository _refreshRepo  = new();
    private readonly FakeIdentityRoleRepository _roleRepo     = new();
    private readonly FakeSmsService             _sms          = new();
    private readonly FakeJwtService             _jwt          = new();
    private readonly IdentityHandlers           _sut;
    private readonly CancellationToken          _ct = CancellationToken.None;

    public NewCustomerLoginScenarioTests()
    {
        _sut = new IdentityHandlers(_userRepo, _refreshRepo, _roleRepo, _otpRepo, _sms, _jwt);
    }

    // ── گام ۱: درخواست OTP ────────────────────────────────────────────────────

    /// <summary>گام ۱: درخواست OTP باید SMS ارسال کند</summary>
    [Fact]
    public async Task Step1_RequestOtp_Should_SendSms_To_Phone()
    {
        var result = await _sut.Handle(new RequestOtpCommand(Phone), _ct);

        Assert.True(result);
        Assert.Equal(Phone, _sms.LastPhone);
        Assert.NotNull(_sms.LastCode);
        Assert.Equal(6, _sms.LastCode!.Length);
    }

    /// <summary>گام ۱: کد OTP باید در دیتابیس ذخیره شود</summary>
    [Fact]
    public async Task Step1_RequestOtp_Should_SaveOtpToRepository()
    {
        await _sut.Handle(new RequestOtpCommand(Phone), _ct);

        Assert.NotNull(_otpRepo.Added);
        Assert.Equal(Phone,              _otpRepo.Added!.PhoneNumber);
        Assert.False(_otpRepo.Added.IsUsed);
        Assert.False(_otpRepo.Added.IsExpired);
    }

    // ── گام ۲: تأیید OTP ──────────────────────────────────────────────────────

    /// <summary>گام ۲: تأیید OTP باید کاربر جدید بسازد</summary>
    [Fact]
    public async Task Step2_VerifyOtp_For_NewUser_Should_CreateUser_Automatically()
    {
        _otpRepo.SeedValidOtp(Phone, Code);

        await _sut.Handle(new VerifyOtpCommand(Phone, Code), _ct);

        Assert.NotNull(_userRepo.Added);
        Assert.Equal(Phone,           _userRepo.Added!.PhoneNumber);
        Assert.Equal(UserType.Customer, _userRepo.Added.UserType);
        Assert.Equal(UserStatus.Active, _userRepo.Added.Status);
    }

    /// <summary>گام ۲: تأیید OTP باید JWT و Refresh Token صادر کند</summary>
    [Fact]
    public async Task Step2_VerifyOtp_Should_IssueJwtAndRefreshToken()
    {
        _otpRepo.SeedValidOtp(Phone, Code);

        var result = await _sut.Handle(new VerifyOtpCommand(Phone, Code), _ct);

        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);
        Assert.True(result.AccessTokenExpiresAt  > DateTime.UtcNow);
        Assert.True(result.RefreshTokenExpiresAt > DateTime.UtcNow);
    }

    /// <summary>گام ۲: تأیید OTP باید Refresh Token را در دیتابیس ذخیره کند</summary>
    [Fact]
    public async Task Step2_VerifyOtp_Should_SaveRefreshTokenToRepository()
    {
        _otpRepo.SeedValidOtp(Phone, Code);

        var result = await _sut.Handle(new VerifyOtpCommand(Phone, Code), _ct);

        Assert.NotNull(_refreshRepo.Added);
        Assert.Equal(result.RefreshToken, _refreshRepo.Added!.Token);
    }

    /// <summary>گام ۲: OTP باید بعد از تأیید IsUsed = true شود</summary>
    [Fact]
    public async Task Step2_VerifyOtp_Should_MarkOtpAsUsed()
    {
        var otp = _otpRepo.SeedValidOtp(Phone, Code);

        await _sut.Handle(new VerifyOtpCommand(Phone, Code), _ct);

        Assert.True(otp.IsUsed);
    }

    // ── سناریوی کامل End-to-End ───────────────────────────────────────────────

    /// <summary>
    /// سناریوی کامل: RequestOtp → VerifyOtp → کاربر جدید + JWT
    /// </summary>
    [Fact]
    public async Task FullScenario_NewCustomer_RequestOtp_Then_VerifyOtp_Should_Succeed()
    {
        // گام ۱: درخواست OTP
        var otpSent = await _sut.Handle(new RequestOtpCommand(Phone), _ct);
        Assert.True(otpSent);

        // کد ارسال‌شده را از Fake SMS می‌گیریم
        var sentCode = _sms.LastCode!;

        // گام ۲: تأیید OTP با کد واقعی
        var response = await _sut.Handle(new VerifyOtpCommand(Phone, sentCode), _ct);

        Assert.NotNull(response.AccessToken);
        Assert.NotNull(response.RefreshToken);
        Assert.Equal(Phone, response.Email); // Email = PhoneNumber
        Assert.NotNull(_userRepo.Added);
        Assert.Equal(Phone, _userRepo.Added!.PhoneNumber);
    }
}
