/// <summary>Fake پیاده‌سازی IUserRepository برای تست‌های Identity</summary>
using Identity.Domain.Enums;
using Identity.Domain.Repositories;
using IdentityUser = Identity.Domain.Aggregates.User;

namespace Ahmad.OnlineShop.Application.Tests.Fakes.Identity;

public class FakeIdentityUserRepository : IUserRepository
{
    private readonly Dictionary<long,   IdentityUser> _byId    = new();
    private readonly Dictionary<string, IdentityUser> _byPhone = new();
    private long _nextId = 1;

    public IdentityUser? Added   { get; private set; }
    public IdentityUser? Updated { get; private set; }

    // ─── Preset users برای سناریوها ─────────────────────────────────────────

    public static IdentityUser ExistingCustomer(long id = 1, string phone = "09121234567") =>
        IdentityUser.Create(id, phone, UserType.Customer);

    public static IdentityUser ExistingSeller(long id = 2, string phone = "09129876543") =>
        IdentityUser.Create(id, phone, UserType.Seller);

    // ─── Interface ───────────────────────────────────────────────────────────

    public Task<IdentityUser?> GetByIdAsync(long id, CancellationToken token = default)
        => Task.FromResult(_byId.GetValueOrDefault(id));

    public Task<IdentityUser?> GetByPhoneAsync(string phoneNumber, CancellationToken token = default)
        => Task.FromResult(_byPhone.GetValueOrDefault(phoneNumber));

    public Task AddAsync(IdentityUser user, CancellationToken token = default)
    {
        Added = user;
        _byId[user.Id]             = user;
        _byPhone[user.PhoneNumber] = user;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(IdentityUser user, CancellationToken token = default)
    {
        Updated = user;
        _byId[user.Id]             = user;
        _byPhone[user.PhoneNumber] = user;
        return Task.CompletedTask;
    }

    public Task<long> GetNextIdAsync() => Task.FromResult(_nextId++);

    public void Seed(IdentityUser user)
    {
        _byId[user.Id]             = user;
        _byPhone[user.PhoneNumber] = user;
    }
}
