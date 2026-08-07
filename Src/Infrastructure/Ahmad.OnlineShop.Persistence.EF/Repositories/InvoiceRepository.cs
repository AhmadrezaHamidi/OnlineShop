using Ahmad.OnlineShop.Domain.Invoice.Aggregates;
using Ahmad.OnlineShop.Domain.Invoice.Entities;
using Ahmad.OnlineShop.Domain.Invoice.Repositories;
using AhmadBase.Persistence.NHiLoHelper;
using Microsoft.EntityFrameworkCore;

namespace Ahmad.OnlineShop.Persistence.EF.Repositories;

public sealed class InvoiceRepository(
    ApplicationDbContext context,
    IHiLoIdGenerator     hiLoGenerator) : IInvoiceRepository
{
    public async Task<Invoice?> GetByIdAsync(long id, CancellationToken token = default)
        => await context.Invoices
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id, token);

    public async Task<Invoice?> GetByOrderIdAsync(long orderId, CancellationToken token = default)
        => await context.Invoices
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.OrderId == orderId, token);

    public async Task<List<Invoice>> GetByUserIdAsync(
        long userId, int page, int pageSize, CancellationToken token = default)
        => await context.Invoices
            .Include(x => x.Items)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.IssuedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(token);

    public Task<int> CountByUserIdAsync(long userId, CancellationToken token = default)
        => context.Invoices.CountAsync(x => x.UserId == userId, token);

    public async Task AddAsync(Invoice invoice, CancellationToken token = default)
        => await context.Invoices.AddAsync(invoice, token);

    public Task UpdateAsync(Invoice invoice, CancellationToken token = default)
    {
        context.Invoices.Update(invoice);
        return Task.CompletedTask;
    }

    public long GetNextId()     => hiLoGenerator.GetNextId<Invoice>();
    public long GetNextItemId() => hiLoGenerator.GetNextId<Invoice>();
}
