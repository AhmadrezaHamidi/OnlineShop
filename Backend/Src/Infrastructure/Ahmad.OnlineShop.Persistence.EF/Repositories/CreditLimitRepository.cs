using Ahmad.OnlineShop.Domain.Bnpl.Aggregates;
using Ahmad.OnlineShop.Domain.Repositories;
using AhmadBase.Persistence.NHiLoHelper;
using Microsoft.EntityFrameworkCore;

namespace Ahmad.OnlineShop.Persistence.EF.Repositories;

public sealed class CreditLimitRepository(
    ApplicationDbContext context,
    IHiLoIdGenerator     hiLoGenerator) : ICreditLimitRepository
{
    public async Task<CreditLimit?> GetByUserIdAsync(long userId, CancellationToken token = default)
        => await context.CreditLimits
            .FirstOrDefaultAsync(x => x.UserId == userId, token);

    public async Task AddAsync(CreditLimit creditLimit, CancellationToken token = default)
        => await context.CreditLimits.AddAsync(creditLimit, token);

    public Task UpdateAsync(CreditLimit creditLimit, CancellationToken token = default)
    {
        context.CreditLimits.Update(creditLimit);
        return Task.CompletedTask;
    }

    public Task<long> GetNextIdAsync()
        => Task.FromResult(hiLoGenerator.GetNextId<CreditLimit>());
}
