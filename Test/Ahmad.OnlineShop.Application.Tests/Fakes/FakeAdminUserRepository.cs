/// <summary>پیاده‌سازی Fake برای IAdminUserRepository جهت استفاده در تست‌ها</summary>
namespace Ahmad.OnlineShop.Application.Tests.Fakes;

public class FakeAdminUserRepository : IAdminUserRepository
{
    private readonly Dictionary<long, AdminUser> _store = new();
    private long _nextId = 1;

    public AdminUser? FoundByEmail { get; set; }
    public AdminUser? Added       { get; private set; }
    public AdminUser? Updated     { get; private set; }

    public Task<AdminUser?> GetByIdAsync(long id, CancellationToken token = default)
        => Task.FromResult(_store.GetValueOrDefault(id));

    public Task<AdminUser?> GetByEmailAsync(string email, CancellationToken token = default)
        => Task.FromResult(FoundByEmail);

    public Task<(List<AdminUser> Items, int Total)> GetListAsync(
        int page, int pageSize,
        Ahmad.OnlineShop.Domain.BackOffice.Enums.AdminStatus? status,
        AdminRole? role,
        CancellationToken token = default)
        => Task.FromResult((_store.Values.ToList(), _store.Count));

    public Task AddAsync(AdminUser admin, CancellationToken token = default)
    {
        Added = admin;
        _store[admin.Id] = admin;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(AdminUser admin, CancellationToken token = default)
    {
        Updated = admin;
        _store[admin.Id] = admin;
        return Task.CompletedTask;
    }

    public long GetNextId() => _nextId++;

    /// <summary>مدیر مشخص را در store قرار می‌دهد تا GetByIdAsync آن را بیابد</summary>
    public void Seed(AdminUser admin) => _store[admin.Id] = admin;
}
