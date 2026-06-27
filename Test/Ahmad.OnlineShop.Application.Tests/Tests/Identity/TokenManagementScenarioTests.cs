/// <summary>
/// سناریوهای مدیریت توکن
/// ─────────────────────────────────────────────────────────────────────────────
/// سناریو ۱: Refresh Token معتبر → توکن جدید صادر شود
/// سناریو ۲: Refresh Token منقضی → InvalidRefreshTokenException
/// سناریو ۳: Refresh Token ابطال‌شده → InvalidRefreshTokenException
/// سناریو ۴: Logout → Refresh Token حذف شود
/// سناریو ۵: Rotation — بعد از Refresh، توکن قدیمی باید حذف شود
/// </summary>
using Ahmad.OnlineShop.Application.Tests.Fakes.Identity;
using Identity.Application.Commands;
using Identity.Application.Handlers;
using IdentityUser = Identity.Domain.Aggregates.User;
using Identity.Domain.Entities;
using Identity.Domain.Exceptions;

namespace Ahmad.OnlineShop.Application.Tests.Identity;

public class TokenManagementScenarioTests
{
    private const string Phone        = "09121234567";
    private const string ValidCode    = "123456";
    private const string RefreshToken = "valid-refresh-token";

    private readonly FakeIdentityUserRepository _userRepo    = new();
    private readonly FakeOtpRepository          _otpRepo     = new();
    private readonly FakeRefreshTokenRepository _refreshRepo = new();
    private readonly FakeIdentityRoleRepository _roleRepo    = new();
    private readonly FakeSmsService             _sms         = new();
    private readonly FakeJwtService             _jwt         = new();
    private readonly IdentityHandlers           _sut;
    private readonly CancellationToken          _ct = CancellationToken.None;

    public TokenManagementScenarioTests()
    {
        _sut = new IdentityHandlers(_userRepo, _refreshRepo, _roleRepo, _otpRepo, _sms, _jwt);
    }

    // ── Helper ──────────────────────────────────────────────────────────────

    private void SeedUserWithToken(long userId = 1, string tokenValue = RefreshToken)
    {
        var user  = FakeIdentityUserRepository.ExistingCustomer(userId, Phone);
        var token = new RefreshToken(1, userId, tokenValue, DateTime.UtcNow.AddDays(30));
        _userRepo.Seed(user);
        _refreshRepo.Seed(token);
    }

    // ── سناریو ۱: Refresh Token معتبر ────────────────────────────────────────

    /// <summary>Refresh Token معتبر باید توکن جدید صادر کند</summary>
    [Fact]
    public async Task Scenario1_ValidRefreshToken_Should_IssueNewTokens()
    {
        SeedUserWithToken();

        var result = await _sut.Handle(new RefreshTokenCommand("access", RefreshToken), _ct);

        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);
        Assert.NotEqual(RefreshToken, result.RefreshToken); // توکن جدید
    }

    /// <summary>Refresh Token معتبر باید UserId صحیح در response داشته باشد</summary>
    [Fact]
    public async Task Scenario1_ValidRefreshToken_Should_Return_CorrectUserId()
    {
        SeedUserWithToken(userId: 1);

        var result = await _sut.Handle(new RefreshTokenCommand("access", RefreshToken), _ct);

        Assert.Equal(1L, result.UserId);
    }

    // ── سناریو ۲: Refresh Token منقضی ────────────────────────────────────────

    /// <summary>Refresh Token منقضی باید InvalidRefreshTokenException بدهد</summary>
    [Fact]
    public async Task Scenario2_ExpiredRefreshToken_Should_Throw_InvalidRefreshTokenException()
    {
        var expiredToken = new RefreshToken(1, 1, "expired-token", DateTime.UtcNow.AddDays(-1));
        var user = FakeIdentityUserRepository.ExistingCustomer(1, Phone);
        _userRepo.Seed(user);
        _refreshRepo.Seed(expiredToken);

        await Assert.ThrowsAsync<InvalidRefreshTokenException>(
            () => _sut.Handle(new RefreshTokenCommand("access", "expired-token"), _ct));
    }

    // ── سناریو ۳: Refresh Token ابطال‌شده ────────────────────────────────────

    /// <summary>Refresh Token ابطال‌شده باید InvalidRefreshTokenException بدهد</summary>
    [Fact]
    public async Task Scenario3_RevokedRefreshToken_Should_Throw_InvalidRefreshTokenException()
    {
        var revokedToken = new RefreshToken(1, 1, "revoked-token", DateTime.UtcNow.AddDays(30));
        revokedToken.Revoke();
        var user = FakeIdentityUserRepository.ExistingCustomer(1, Phone);
        _userRepo.Seed(user);
        _refreshRepo.Seed(revokedToken);

        await Assert.ThrowsAsync<InvalidRefreshTokenException>(
            () => _sut.Handle(new RefreshTokenCommand("access", "revoked-token"), _ct));
    }

    // ── سناریو ۴: Logout ──────────────────────────────────────────────────────

    /// <summary>Logout باید Refresh Token را از دیتابیس حذف کند</summary>
    [Fact]
    public async Task Scenario4_Logout_Should_DeleteRefreshToken()
    {
        SeedUserWithToken();

        var result = await _sut.Handle(new LogoutCommand(RefreshToken), _ct);

        Assert.True(result);
        var deleted = await _refreshRepo.GetByTokenAsync(RefreshToken);
        Assert.Null(deleted);
    }

    /// <summary>Logout با توکن نامعتبر باید InvalidRefreshTokenException بدهد</summary>
    [Fact]
    public async Task Scenario4_Logout_WithInvalidToken_Should_Throw_InvalidRefreshTokenException()
    {
        await Assert.ThrowsAsync<InvalidRefreshTokenException>(
            () => _sut.Handle(new LogoutCommand("non-existent-token"), _ct));
    }

    // ── سناریو ۵: Token Rotation ─────────────────────────────────────────────

    /// <summary>بعد از Refresh، توکن قدیمی باید حذف شده باشد (rotation)</summary>
    [Fact]
    public async Task Scenario5_AfterRefresh_OldToken_Should_Be_Deleted()
    {
        SeedUserWithToken();

        await _sut.Handle(new RefreshTokenCommand("access", RefreshToken), _ct);

        var oldToken = await _refreshRepo.GetByTokenAsync(RefreshToken);
        Assert.Null(oldToken); // توکن قدیمی حذف شده
    }

    /// <summary>بعد از Refresh، توکن جدید باید در دیتابیس ذخیره شده باشد</summary>
    [Fact]
    public async Task Scenario5_AfterRefresh_NewToken_Should_Be_Saved()
    {
        SeedUserWithToken();

        var result = await _sut.Handle(new RefreshTokenCommand("access", RefreshToken), _ct);

        var newToken = await _refreshRepo.GetByTokenAsync(result.RefreshToken);
        Assert.NotNull(newToken);
    }

    // ── سناریوی End-to-End: Login → Refresh → Logout ─────────────────────────

    /// <summary>
    /// سناریوی کامل: VerifyOtp → RefreshToken → Logout
    /// </summary>
    [Fact]
    public async Task FullScenario_Login_Then_Refresh_Then_Logout()
    {
        // گام ۱: ورود
        _otpRepo.SeedValidOtp(Phone, ValidCode);
        var loginResult = await _sut.Handle(new VerifyOtpCommand(Phone, ValidCode), _ct);
        var userId      = loginResult.UserId;
        var rt1         = loginResult.RefreshToken;

        // گام ۲: تجدید توکن
        var refreshResult = await _sut.Handle(new RefreshTokenCommand(loginResult.AccessToken, rt1), _ct);
        var rt2           = refreshResult.RefreshToken;
        Assert.NotEqual(rt1, rt2); // توکن جدید

        // گام ۳: خروج
        var logoutResult = await _sut.Handle(new LogoutCommand(rt2), _ct);
        Assert.True(logoutResult);

        // توکن دیگر نباید قابل استفاده باشد
        await Assert.ThrowsAsync<InvalidRefreshTokenException>(
            () => _sut.Handle(new RefreshTokenCommand(refreshResult.AccessToken, rt2), _ct));
    }
}
