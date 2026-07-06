/// <summary>
/// تست‌های Entity توکن تجدید (RefreshToken)
/// پوشش‌دهنده: ایجاد، اعتبارسنجی، ابطال
/// خطاهای تست‌شده: InvalidRefreshTokenException (منقضی یا ابطال‌شده)
/// </summary>
namespace Ahmad.OnlineShop.Domain.Identity.Tests;

public class RefreshTokenTests
{
    // ── Constructor ──────────────────────────────────────────────────────────

    /// <summary>سازنده باید تمام مشخصات را ست کند و IsRevoked=false باشد</summary>
    [Fact]
    public void Constructor_Should_Set_All_Properties()
    {
        var expiry = DateTime.UtcNow.AddDays(30);
        var token  = new RefreshToken(1, 100, "raw-token-value", expiry);

        Assert.Equal(1,                 token.Id);
        Assert.Equal(100,               token.UserId);
        Assert.Equal("raw-token-value", token.Token);
        Assert.Equal(expiry,            token.ExpiresAt);
        Assert.False(token.IsRevoked);
    }

    // ── EnsureValid ───────────────────────────────────────────────────────────

    /// <summary>توکن معتبر نباید خطا بدهد</summary>
    [Fact]
    public void EnsureValid_When_Valid_Should_Not_Throw()
    {
        var token = new RefreshToken(1, 100, "token", DateTime.UtcNow.AddDays(1));

        var ex = Record.Exception(() => token.EnsureValid());

        Assert.Null(ex);
    }

    /// <summary>خطا: توکن منقضی‌شده → InvalidRefreshTokenException</summary>
    [Fact]
    public void EnsureValid_When_Expired_Should_Throw_InvalidRefreshTokenException()
    {
        var token = new RefreshToken(1, 100, "token", DateTime.UtcNow.AddDays(-1));

        Assert.Throws<InvalidRefreshTokenException>(() => token.EnsureValid());
    }

    /// <summary>خطا: توکن ابطال‌شده → InvalidRefreshTokenException</summary>
    [Fact]
    public void EnsureValid_When_Revoked_Should_Throw_InvalidRefreshTokenException()
    {
        var token = new RefreshToken(1, 100, "token", DateTime.UtcNow.AddDays(1));
        token.Revoke();

        Assert.Throws<InvalidRefreshTokenException>(() => token.EnsureValid());
    }

    // ── Revoke ────────────────────────────────────────────────────────────────

    /// <summary>ابطال باید IsRevoked را True کند</summary>
    [Fact]
    public void Revoke_Should_Set_IsRevoked_True()
    {
        var token = new RefreshToken(1, 100, "token", DateTime.UtcNow.AddDays(1));
        token.Revoke();

        Assert.True(token.IsRevoked);
    }

    /// <summary>ابطال دوباره نباید خطا بدهد و IsRevoked=true بماند</summary>
    [Fact]
    public void Revoke_When_AlreadyRevoked_Should_Stay_Revoked()
    {
        var token = new RefreshToken(1, 100, "token", DateTime.UtcNow.AddDays(1));
        token.Revoke();
        token.Revoke();

        Assert.True(token.IsRevoked);
    }
}
