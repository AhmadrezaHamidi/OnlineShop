/// <summary>Fake پیاده‌سازی IJwtService، ISmsService، IRefreshTokenRepository، IRoleRepository</summary>
using Identity.Application.Services;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using IdentityUser = Identity.Domain.Aggregates.User;

namespace Ahmad.OnlineShop.Application.Tests.Fakes.Identity;

// ── IJwtService ──────────────────────────────────────────────────────────────

public class FakeJwtService : IJwtService
{
    public string LastGeneratedAccessToken  { get; private set; } = "fake-access-token";
    public string LastGeneratedRefreshToken { get; private set; } = "fake-refresh-token";

    public (string Token, DateTime ExpiresAt) GenerateAccessToken(IdentityUser user)
    {
        LastGeneratedAccessToken = $"access-{user.PhoneNumber}-{DateTime.UtcNow.Ticks}";
        return (LastGeneratedAccessToken, DateTime.UtcNow.AddHours(1));
    }

    public string GenerateRefreshToken()
    {
        LastGeneratedRefreshToken = $"refresh-{Guid.NewGuid()}";
        return LastGeneratedRefreshToken;
    }
}

// ── ISmsService ───────────────────────────────────────────────────────────────

public class FakeSmsService : ISmsService
{
    public string? LastPhone  { get; private set; }
    public string? LastCode   { get; private set; }
    public bool    ShouldFail { get; set; }

    public Task<bool> SendOtpAsync(string phoneNumber, string code, CancellationToken token = default)
    {
        LastPhone = phoneNumber;
        LastCode  = code;
        return Task.FromResult(!ShouldFail);
    }

    public Task<bool> SendBulkAsync(IReadOnlyList<string> phones, string message, CancellationToken token = default)
        => Task.FromResult(!ShouldFail);
}

// ── IRefreshTokenRepository ───────────────────────────────────────────────────

public class FakeRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly Dictionary<string, RefreshToken> _store = new();
    private long _nextId = 1;

    public RefreshToken? Added { get; private set; }

    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        => Task.FromResult(_store.GetValueOrDefault(token));

    public Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(long userId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<RefreshToken>>(
            _store.Values.Where(t => t.UserId == userId && !t.IsRevoked).ToList());

    public Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        Added = refreshToken;
        _store[refreshToken.Token] = refreshToken;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        _store.Remove(refreshToken.Token);
        return Task.CompletedTask;
    }

    public Task DeleteAllForUserAsync(long userId, CancellationToken ct = default)
    {
        var keys = _store.Where(p => p.Value.UserId == userId).Select(p => p.Key).ToList();
        foreach (var k in keys) _store.Remove(k);
        return Task.CompletedTask;
    }

    public Task<long> GetNextIdAsync() => Task.FromResult(_nextId++);

    public void Seed(RefreshToken token) => _store[token.Token] = token;
}

// ── IRoleRepository ───────────────────────────────────────────────────────────

public class FakeIdentityRoleRepository : IRoleRepository
{
    private readonly Dictionary<long, Role> _store = new();

    public void Seed(Role role) => _store[role.Id] = role;

    public Task<Role?>               GetByIdAsync(long id, CancellationToken token = default)
        => Task.FromResult(_store.GetValueOrDefault(id));

    public Task<Role?>               GetByNameAsync(string name, CancellationToken token = default)
        => Task.FromResult(_store.Values.FirstOrDefault(r => r.Name == name));

    public Task<bool>                ExistsByNameAsync(string name, CancellationToken token = default)
        => Task.FromResult(_store.Values.Any(r => r.Name == name));

    public Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken token = default)
        => Task.FromResult<IReadOnlyList<Role>>(_store.Values.ToList());

    public Task AddAsync(Role role, CancellationToken token = default)
        { _store[role.Id] = role; return Task.CompletedTask; }

    public Task UpdateAsync(Role role, CancellationToken token = default)
        { _store[role.Id] = role; return Task.CompletedTask; }

    public Task<long> GetNextIdAsync() => Task.FromResult((long)(_store.Count + 1));
}
