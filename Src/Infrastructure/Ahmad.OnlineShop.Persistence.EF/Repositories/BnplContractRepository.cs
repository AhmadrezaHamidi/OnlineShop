using Ahmad.OnlineShop.Domain.Bnpl.Aggregates;
using Ahmad.OnlineShop.Domain.Repositories;
using AhmadBase.Persistence.NHiLoHelper;
using Microsoft.EntityFrameworkCore;

namespace Ahmad.OnlineShop.Persistence.EF.Repositories;

public sealed class BnplContractRepository(
    ApplicationDbContext context,
    IHiLoIdGenerator hiLoGenerator) : IBnplContractRepository
{
    public async Task<BnplContract?> GetByIdAsync(long id, CancellationToken token = default)
        => await context.BnplContracts
            .Include(x => x.Installments)
            .FirstOrDefaultAsync(x => x.Id == id, token);

    public async Task<(List<BnplContract> Items, int Total)> GetByUserIdAsync(
        long userId, int page, int pageSize, CancellationToken token = default)
    {
        var query = context.BnplContracts
            .Include(x => x.Installments)
            .Where(x => x.UserId == userId);

        var total = await query.CountAsync(token);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(token);

        return (items, total);
    }

    public async Task AddAsync(BnplContract contract, CancellationToken token = default)
        => await context.BnplContracts.AddAsync(contract, token);

    public Task UpdateAsync(BnplContract contract, CancellationToken token = default)
    {
        context.BnplContracts.Update(contract);
        return Task.CompletedTask;
    }

    public Task<long> GetNextIdAsync()
        => Task.FromResult(hiLoGenerator.GetNextId<BnplContract>());

    public Task<long> GetNextInstallmentIdAsync()
        => Task.FromResult(hiLoGenerator.GetNextId<BnplContract>());
}
