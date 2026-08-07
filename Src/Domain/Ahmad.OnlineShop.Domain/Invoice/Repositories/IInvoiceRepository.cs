using InvoiceAgg = Ahmad.OnlineShop.Domain.Invoice.Aggregates.Invoice;

namespace Ahmad.OnlineShop.Domain.Invoice.Repositories;

public interface IInvoiceRepository
{
    Task<InvoiceAgg?>        GetByIdAsync(long id, CancellationToken token = default);
    Task<InvoiceAgg?>        GetByOrderIdAsync(long orderId, CancellationToken token = default);
    Task<List<InvoiceAgg>>   GetByUserIdAsync(long userId, int page, int pageSize, CancellationToken token = default);
    Task<int>                CountByUserIdAsync(long userId, CancellationToken token = default);
    Task                     AddAsync(InvoiceAgg invoice, CancellationToken token = default);
    Task                     UpdateAsync(InvoiceAgg invoice, CancellationToken token = default);
    long                     GetNextId();
    long                     GetNextItemId();
}
