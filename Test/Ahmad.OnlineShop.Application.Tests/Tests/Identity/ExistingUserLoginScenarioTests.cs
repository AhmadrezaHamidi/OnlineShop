/// <summary>
/// سناریو: ورود کاربران موجود (مشتری و فروشنده)
/// ─────────────────────────────────────────────────────────────────────────────
/// - مشتری موجود با OTP وارد می‌شود (کاربر جدید ساخته نمی‌شود)
/// - فروشنده موجود با OTP وارد می‌شود
/// - VerifyOtp برای هر دو نوع کاربر JWT صادر می‌کند
/// </summary>
using Ahmad.OnlineShop.Application.Tests.Fakes.Identity;
using Identity.Application.Commands;
using Identity.Application.Handlers;
using IdentityUser = Identity.Domain.Aggregates.User;
using Identity.Domain.Enums;

namespace Ahmad.OnlineShop.Application.Tests.Identity;

public class ExistingUserLoginScenarioTests
{
    private const string CustomerPhone = "09121234567";
    private const string SellerPhone   = "09129876543";
    private const string ValidCode     = "123456";

    private readonly FakeIdentityUserRepository _userRepo    = new();
    private readonly FakeOtpRepository          _otpRepo     = new();
    private readonly FakeRefreshTokenRepository _refreshRepo = new();
    private readonly FakeIdentityRoleRepository _roleRepo    = new();
    private readonly FakeSmsService             _sms         = new();
    private readonly FakeJwtService             _jwt         = new();
    private readonly IdentityHandlers           _sut;
    private readonly CancellationToken          _ct = CancellationToken.None;

    public ExistingUserLoginScenarioTests()
    {
        _sut = new IdentityHandlers(_userRepo, _refreshRepo, _roleRepo, _otpRepo, _sms, _jwt);
    }

    // ── سناریو: مشتری موجود ──────────────────────────────────────────────────

    /// <summary>مشتری موجود باید با OTP وارد شود بدون ایجاد کاربر جدید</summary>
    [Fact]
    public async Task ExistingCustomer_VerifyOtp_Should_NOT_Create_NewUser()
    {
        var customer = FakeIdentityUserRepository.ExistingCustomer(1, CustomerPhone);
        _userRepo.Seed(customer);
        _otpRepo.SeedValidOtp(CustomerPhone, ValidCode);

        await _sut.Handle(new VerifyOtpCommand(CustomerPhone, ValidCode), _ct);

        // کاربر جدید ساخته نشده
        Assert.Null(_userRepo.Added);
        // کاربر قدیمی آپدیت نشده
        Assert.Null(_userRepo.Updated);
    }

    /// <summary>مشتری موجود باید JWT دریافت کند</summary>
    [Fact]
    public async Task ExistingCustomer_VerifyOtp_Should_IssueJwt()
    {
        var customer = FakeIdentityUserRepository.ExistingCustomer(1, CustomerPhone);
        _userRepo.Seed(customer);
        _otpRepo.SeedValidOtp(CustomerPhone, ValidCode);

        var result = await _sut.Handle(new VerifyOtpCommand(CustomerPhone, ValidCode), _ct);

        Assert.Equal(1L,            result.UserId);
        Assert.Equal(CustomerPhone, result.Email);
        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);
    }

    // ── سناریو: فروشنده موجود ────────────────────────────────────────────────

    /// <summary>فروشنده موجود باید با OTP وارد شود</summary>
    [Fact]
    public async Task ExistingSeller_VerifyOtp_Should_Login_Successfully()
    {
        var seller = FakeIdentityUserRepository.ExistingSeller(2, SellerPhone);
        _userRepo.Seed(seller);
        _otpRepo.SeedValidOtp(SellerPhone, ValidCode);

        var result = await _sut.Handle(new VerifyOtpCommand(SellerPhone, ValidCode), _ct);

        Assert.Equal(2L,          result.UserId);
        Assert.Equal(SellerPhone, result.Email);
        Assert.NotNull(result.AccessToken);
    }

    /// <summary>فروشنده باید UserType.Seller داشته باشد</summary>
    [Fact]
    public async Task ExistingSeller_Should_HaveSellerUserType()
    {
        var seller = FakeIdentityUserRepository.ExistingSeller(2, SellerPhone);
        _userRepo.Seed(seller);
        _otpRepo.SeedValidOtp(SellerPhone, ValidCode);

        await _sut.Handle(new VerifyOtpCommand(SellerPhone, ValidCode), _ct);

        var foundSeller = await _userRepo.GetByPhoneAsync(SellerPhone);
        Assert.Equal(UserType.Seller, foundSeller!.UserType);
    }

    // ── سناریو: RequestOtp برای کاربر موجود ─────────────────────────────────

    /// <summary>درخواست OTP برای کاربر موجود باید SMS جدید ارسال کند</summary>
    [Fact]
    public async Task ExistingUser_RequestOtp_Should_SendNewSms()
    {
        var customer = FakeIdentityUserRepository.ExistingCustomer(1, CustomerPhone);
        _userRepo.Seed(customer);

        var result = await _sut.Handle(new RequestOtpCommand(CustomerPhone), _ct);

        Assert.True(result);
        Assert.Equal(CustomerPhone, _sms.LastPhone);
        Assert.NotNull(_sms.LastCode);
    }

    // ── سناریوی End-to-End: مشتری موجود ─────────────────────────────────────

    /// <summary>
    /// سناریوی کامل: مشتری موجود → RequestOtp → VerifyOtp → JWT
    /// </summary>
    [Fact]
    public async Task FullScenario_ExistingCustomer_Login_E2E()
    {
        // آماده‌سازی: کاربر موجود
        var customer = FakeIdentityUserRepository.ExistingCustomer(1, CustomerPhone);
        _userRepo.Seed(customer);

        // گام ۱
        await _sut.Handle(new RequestOtpCommand(CustomerPhone), _ct);
        var code = _sms.LastCode!;

        // گام ۲
        var response = await _sut.Handle(new VerifyOtpCommand(CustomerPhone, code), _ct);

        Assert.NotNull(response.AccessToken);
        Assert.NotNull(response.RefreshToken);
        Assert.Equal(1L, response.UserId);
        Assert.Null(_userRepo.Added); // کاربر جدید ساخته نشده
    }
}
