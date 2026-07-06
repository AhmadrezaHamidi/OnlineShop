/// <summary>پیاده‌سازی Fake برای IBnplContractRepository و ICreditLimitRepository</summary>
namespace Ahmad.OnlineShop.Application.Tests.Fakes;

public class FakeBnplContractRepository : IBnplContractRepository
{
    private readonly Dictionary<long, BnplContract> _store = new();
    private long _nextId = 1;

    public BnplContract? Added   { get; private set; }
    public BnplContract? Updated { get; private set; }

    public Task<BnplContract?> GetByIdAsync(long id, CancellationToken token = default)
        => Task.FromResult(_store.GetValueOrDefault(id));

    public Task<(List<BnplContract> Items, int Total)> GetByUserIdAsync(
        long userId, int page, int pageSize, CancellationToken token = default)
    {
        var items = _store.Values.Where(c => c.UserId == userId).ToList();
        return Task.FromResult((items, items.Count));
    }

    public Task AddAsync(BnplContract contract, CancellationToken token = default)
    {
        Added = contract;
        _store[contract.Id] = contract;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(BnplContract contract, CancellationToken token = default)
    {
        Updated = contract;
        _store[contract.Id] = contract;
        return Task.CompletedTask;
    }

    public Task<long> GetNextIdAsync()        => Task.FromResult(_nextId++);
    public Task<long> GetNextInstallmentIdAsync() => Task.FromResult(_nextId++);

    public void Seed(BnplContract contract)   => _store[contract.Id] = contract;
}

public class FakeCreditLimitRepository : ICreditLimitRepository
{
    private readonly Dictionary<long, CreditLimit> _byUser = new();
    private long _nextId = 1;

    public CreditLimit? Updated { get; private set; }

    public Task<CreditLimit?> GetByUserIdAsync(long userId, CancellationToken token = default)
        => Task.FromResult(_byUser.GetValueOrDefault(userId));

    public Task AddAsync(CreditLimit credit, CancellationToken token = default)
    {
        _byUser[credit.UserId] = credit;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(CreditLimit credit, CancellationToken token = default)
    {
        Updated = credit;
        _byUser[credit.UserId] = credit;
        return Task.CompletedTask;
    }

    public Task<long> GetNextIdAsync() => Task.FromResult(_nextId++);

    public void Seed(CreditLimit credit) => _byUser[credit.UserId] = credit;
}
