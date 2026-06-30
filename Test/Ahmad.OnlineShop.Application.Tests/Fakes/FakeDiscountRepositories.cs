/// <summary>پیاده‌سازی Fake برای IDiscountRepository و IProductPackageRepository</summary>
using Ahmad.OnlineShop.Domain.Discount.Repositories;
using DiscountAggregate = Ahmad.OnlineShop.Domain.Discount.Aggregates.Discount;
using PackageAgg        = Ahmad.OnlineShop.Domain.Discount.Aggregates.ProductPackage;

namespace Ahmad.OnlineShop.Application.Tests.Fakes;

public class FakeDiscountRepository : IDiscountRepository
{
    private readonly Dictionary<long, DiscountAggregate> _store = new();
    private long _nextId = 1;

    public bool              CodeExists { get; set; }
    public DiscountAggregate? Added     { get; private set; }
    public DiscountAggregate? Updated   { get; private set; }

    public Task<DiscountAggregate?> GetByIdAsync(long id, CancellationToken token = default)
        => Task.FromResult(_store.GetValueOrDefault(id));

    public Task<DiscountAggregate?> GetByCodeAsync(string code, CancellationToken token = default)
        => Task.FromResult(_store.Values.FirstOrDefault(d => d.Code == code.ToUpperInvariant()));

    public Task<bool> CodeExistsAsync(string code, CancellationToken token = default)
        => Task.FromResult(CodeExists);

    public Task<(List<DiscountAggregate> Items, int Total)> GetListAsync(
        int page, int pageSize, bool? isActive, CancellationToken token = default)
    {
        var items = _store.Values.ToList();
        return Task.FromResult((items, items.Count));
    }

    public Task AddAsync(DiscountAggregate discount, CancellationToken token = default)
    {
        Added = discount;
        _store[discount.Id] = discount;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(DiscountAggregate discount, CancellationToken token = default)
    {
        Updated = discount;
        _store[discount.Id] = discount;
        return Task.CompletedTask;
    }

    public long GetNextId() => _nextId++;

    /// <summary>داده اولیه برای تست‌های GetById/GetByCode</summary>
    public void Seed(DiscountAggregate discount) => _store[discount.Id] = discount;
}

public class FakeProductPackageRepository : IProductPackageRepository
{
    private readonly Dictionary<long, PackageAgg> _store = new();
    private long _nextId = 1;

    public PackageAgg? Added   { get; private set; }
    public PackageAgg? Updated { get; private set; }

    public Task<PackageAgg?> GetByIdAsync(long id, CancellationToken token = default)
        => Task.FromResult(_store.GetValueOrDefault(id));

    public Task<(List<PackageAgg> Items, int Total)> GetListAsync(
        int page, int pageSize, bool? isActive, CancellationToken token = default)
    {
        var items = _store.Values.ToList();
        return Task.FromResult((items, items.Count));
    }

    public Task AddAsync(PackageAgg package, CancellationToken token = default)
    {
        Added = package;
        _store[package.Id] = package;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(PackageAgg package, CancellationToken token = default)
    {
        Updated = package;
        _store[package.Id] = package;
        return Task.CompletedTask;
    }

    public long GetNextId()     => _nextId++;
    public long GetNextItemId() => _nextId++ * 100 + 1;

    /// <summary>داده اولیه برای تست‌های GetById</summary>
    public void Seed(PackageAgg package) => _store[package.Id] = package;
}
