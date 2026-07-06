using AhmadBase.Persistence.NHiLoHelper;
using Identity.Domain.Aggregates;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ahmad.OnlineShop.Persistence.EF.Repositories;

// ── User ─────────────────────────────────────────────────────────────────────

public sealed class IdentityUserRepository(
    IdentityAppDbContext context,
    IHiLoIdGenerator     hiLoGenerator) : IUserRepository
{
    public Task<User?> GetByIdAsync(long id, CancellationToken token = default)
        => context.IdentityUsers.FirstOrDefaultAsync(u => u.Id == id, token);

    public Task<User?> GetByPhoneAsync(string phoneNumber, CancellationToken token = default)
        => context.IdentityUsers.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, token);

    public async Task AddAsync(User user, CancellationToken token = default)
        => await context.IdentityUsers.AddAsync(user, token);

    public Task UpdateAsync(User user, CancellationToken token = default)
    {
        context.IdentityUsers.Update(user);
        return Task.CompletedTask;
    }

    // User یک Aggregate است — HiLo استفاده می‌کند
    public Task<long> GetNextIdAsync() => Task.FromResult(hiLoGenerator.GetNextId<User>());
}

// ── OTP ──────────────────────────────────────────────────────────────────────

public sealed class IdentityOtpRepository(IdentityAppDbContext context) : IOtpRepository
{
    public Task<OtpRequest?> GetLatestByPhoneAsync(string phoneNumber, CancellationToken token = default)
        => context.OtpRequests
                  .Where(o => o.PhoneNumber == phoneNumber)
                  .OrderByDescending(o => o.Id)
                  .FirstOrDefaultAsync(token);

    public async Task AddAsync(OtpRequest otp, CancellationToken token = default)
        => await context.OtpRequests.AddAsync(otp, token);

    public Task UpdateAsync(OtpRequest otp, CancellationToken token = default)
    {
        context.OtpRequests.Update(otp);
        return Task.CompletedTask;
    }

    // OtpRequest entity ساده است — MAX(Id)+1 از DB
    public async Task<long> GetNextIdAsync()
    {
        var max = await context.OtpRequests.MaxAsync(o => (long?)o.Id) ?? 0;
        return max + 1;
    }
}

// ── Refresh Token ─────────────────────────────────────────────────────────────

public sealed class IdentityRefreshTokenRepository(IdentityAppDbContext context) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        => context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token, ct);

    public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(long userId, CancellationToken ct = default)
        => await context.RefreshTokens
                        .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow)
                        .ToListAsync(ct);

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default)
        => await context.RefreshTokens.AddAsync(refreshToken, ct);

    public Task DeleteAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        context.RefreshTokens.Remove(refreshToken);
        return Task.CompletedTask;
    }

    public Task DeleteAllForUserAsync(long userId, CancellationToken ct = default)
    {
        context.RefreshTokens.RemoveRange(context.RefreshTokens.Where(rt => rt.UserId == userId));
        return Task.CompletedTask;
    }

    // RefreshToken entity ساده است — MAX(Id)+1 از DB
    public async Task<long> GetNextIdAsync()
    {
        var max = await context.RefreshTokens.MaxAsync(rt => (long?)rt.Id) ?? 0;
        return max + 1;
    }
}

// ── Role ─────────────────────────────────────────────────────────────────────

public sealed class IdentityRoleRepository(IdentityAppDbContext context) : IRoleRepository
{
    public Task<Role?> GetByIdAsync(long id, CancellationToken token = default)
        => context.Roles.FirstOrDefaultAsync(r => r.Id == id, token);

    public Task<Role?> GetByNameAsync(string name, CancellationToken token = default)
        => context.Roles.FirstOrDefaultAsync(r => r.Name == name, token);

    public Task<bool> ExistsByNameAsync(string name, CancellationToken token = default)
        => context.Roles.AnyAsync(r => r.Name == name, token);

    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken token = default)
        => await context.Roles.ToListAsync(token);

    public async Task AddAsync(Role role, CancellationToken token = default)
        => await context.Roles.AddAsync(role, token);

    public Task UpdateAsync(Role role, CancellationToken token = default)
    {
        context.Roles.Update(role);
        return Task.CompletedTask;
    }

    public async Task<long> GetNextIdAsync()
    {
        var max = await context.Roles.MaxAsync(r => (long?)r.Id) ?? 0;
        return max + 1;
    }
}
