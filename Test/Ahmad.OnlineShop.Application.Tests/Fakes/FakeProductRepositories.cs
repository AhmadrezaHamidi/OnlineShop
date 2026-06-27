/// <summary>پیاده‌سازی Fake برای IProductRepository و ICategoryRepository</summary>
namespace Ahmad.OnlineShop.Application.Tests.Fakes;

public class FakeProductRepository : IProductRepository
{
    private readonly Dictionary<long, ProductAgg> _store = new();
    private long _nextId = 1;

    public ProductAgg? Added   { get; private set; }
    public ProductAgg? Updated { get; private set; }

    public Task<ProductAgg?> Get(long id, CancellationToken token = default)
        => Task.FromResult(_store.GetValueOrDefault(id));

    public Task<(List<ProductAgg> Items, int Total)> GetListAsync(
        int page, int pageSize, string? search, long? categoryId,
        Ahmad.OnlineShop.Domain.Products.Enums.ProductStatus? status, CancellationToken token = default)
    {
        var items = _store.Values.ToList();
        return Task.FromResult((items, items.Count));
    }

    public Task AddAsync(ProductAgg product, CancellationToken token = default)
    {
        Added = product;
        _store[product.Id] = product;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(ProductAgg product, CancellationToken token = default)
    {
        Updated = product;
        _store[product.Id] = product;
        return Task.CompletedTask;
    }

    public long GetNextId() => _nextId++;

    public void Seed(ProductAgg product) => _store[product.Id] = product;
}

public class FakeCategoryRepository : ICategoryRepository
{
    private readonly Dictionary<long, CategoryAgg> _store = new();
    private long _nextId = 1;

    public bool NameExists    { get; set; }
    public CategoryAgg? Added  { get; private set; }
    public CategoryAgg? Updated { get; private set; }

    public Task<CategoryAgg?> Get(long id, CancellationToken token = default)
        => Task.FromResult(_store.GetValueOrDefault(id));

    public Task<List<CategoryAgg>> Gets(CancellationToken token = default)
        => Task.FromResult(_store.Values.ToList());

    public Task AddAsync(CategoryAgg category, CancellationToken token = default)
    {
        Added = category;
        _store[category.Id] = category;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(CategoryAgg category, CancellationToken token = default)
    {
        Updated = category;
        _store[category.Id] = category;
        return Task.CompletedTask;
    }

    public Task<bool> ExistsByName(string name, CancellationToken token = default)
        => Task.FromResult(NameExists);

    public long GetNextId() => _nextId++;

    public Task Add(CategoryAgg category, CancellationToken token = default)   => AddAsync(category, token);
    public Task Update(CategoryAgg category, CancellationToken token = default) => UpdateAsync(category, token);
    public Task<bool> ExistsByNameAsync(string name, CancellationToken token = default) => ExistsByName(name, token);
    public Task<long> GetNextIdAsync() => Task.FromResult(GetNextId());

    public void Seed(CategoryAgg category) => _store[category.Id] = category;
}
