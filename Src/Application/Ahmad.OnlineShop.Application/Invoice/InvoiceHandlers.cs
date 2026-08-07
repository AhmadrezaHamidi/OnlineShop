using Ahmad.OnlineShop.Application.Contract.Invoice.Commands;
using Ahmad.OnlineShop.Application.Query.Contract.Invoice;
using Ahmad.OnlineShop.Domain.Invoice.Aggregates;
using Ahmad.OnlineShop.Domain.Invoice.Repositories;
using Ahmad.OnlineShop.Domain.Order.Enums;
using Ahmad.OnlineShop.Domain.Repositories;
using Ahmad.OnlineShop.Persistence.EF;
using AhmadBase.Application;
using AhmadBase.Application.Query;
using Microsoft.Extensions.Logging;

namespace Ahmad.OnlineShop.Application.Handlers;

public sealed class InvoiceHandlers(
    IInvoiceRepository  invoiceRepository,
    IOrderRepository    orderRepository,
    ApplicationDbContext context,
    ILogger<InvoiceHandlers>? logger = null) :
    ICommandHandler<CreateInvoiceCommand, long>
{
    public async Task<long> Handle(CreateInvoiceCommand command, CancellationToken token)
    {
        // ابتدا بررسی می‌کنیم فاکتور تکراری نباشد
        var existing = await invoiceRepository.GetByOrderIdAsync(command.OrderId, token);
        if (existing is not null)
            return existing.Id;

        var order = await orderRepository.GetByIdAsync(command.OrderId, token)
            ?? throw new InvalidOperationException($"سفارش {command.OrderId} یافت نشد");

        var payment = order.Payments.FirstOrDefault(p =>
            p.Status == Domain.Order.Enums.PaymentStatus.Completed);

        var invoiceId = invoiceRepository.GetNextId();

        var items = order.Items.Select(item => new InvoiceItemArg(
            ItemId:      invoiceRepository.GetNextItemId(),
            ProductId:   item.ProductId,
            ProductName: $"محصول {item.ProductId}",
            Quantity:    item.Quantity,
            UnitPrice:   item.UnitPrice
        )).ToList();

        var subTotal = order.SubTotal > 0 ? order.SubTotal : order.TotalAmount;

        var arg = new CreateInvoiceArg(
            Id:               invoiceId,
            OrderId:          order.Id,
            UserId:           order.UserId,
            SubTotal:         subTotal,
            DiscountAmount:   order.DiscountAmount,
            PackageDiscount:  order.PackageDiscount,
            TotalAmount:      order.TotalAmount,
            DiscountCode:     order.DiscountCode,
            AppliedPackageId: order.AppliedPackageId,
            PaymentMethod:    payment?.Method ?? PaymentMethod.CashOnDelivery,
            TransactionCode:  payment?.TransactionCode,
            Items:            items
        );

        var invoice = Invoice.Create(arg);
        await invoiceRepository.AddAsync(invoice, token);
        await context.SaveChangesAsync(token);

        logger?.LogInformation("فاکتور {InvoiceNumber} برای سفارش {OrderId} صادر شد",
            invoice.InvoiceNumber, command.OrderId);

        return invoice.Id;
    }
}

// ── Query Handlers ────────────────────────────────────────────────────────────

public sealed class InvoiceQueryHandlers(IInvoiceRepository repository) :
    IQueryHandler<GetInvoiceByIdQuery,      GetInvoiceResponse?>,
    IQueryHandler<GetInvoiceByOrderIdQuery, GetInvoiceResponse?>,
    IQueryHandler<GetMyInvoicesQuery,       PagedResult<GetInvoiceResponse>>
{
    public async Task<GetInvoiceResponse?> HandleAsync(GetInvoiceByIdQuery query, CancellationToken token)
    {
        var inv = await repository.GetByIdAsync(query.InvoiceId, token);
        return inv is null ? null : inv.ToResponse();
    }

    public async Task<GetInvoiceResponse?> HandleAsync(GetInvoiceByOrderIdQuery query, CancellationToken token)
    {
        var inv = await repository.GetByOrderIdAsync(query.OrderId, token);
        return inv is null ? null : inv.ToResponse();
    }

    public async Task<PagedResult<GetInvoiceResponse>> HandleAsync(GetMyInvoicesQuery query, CancellationToken token)
    {
        var total = await repository.CountByUserIdAsync(query.UserId, token);
        var items = await repository.GetByUserIdAsync(query.UserId, query.Page, query.PageSize, token);
        return new PagedResult<GetInvoiceResponse>(
            items.Select(i => i.ToResponse()).ToList(),
            total, query.Page, query.PageSize);
    }
}

file static class InvoiceMapper
{
    internal static GetInvoiceResponse ToResponse(this Domain.Invoice.Aggregates.Invoice inv)
        => new(
            Id:               inv.Id,
            OrderId:          inv.OrderId,
            UserId:           inv.UserId,
            InvoiceNumber:    inv.InvoiceNumber,
            Status:           inv.Status,
            SubTotal:         inv.SubTotal,
            DiscountAmount:   inv.DiscountAmount,
            PackageDiscount:  inv.PackageDiscount,
            TotalAmount:      inv.TotalAmount,
            DiscountCode:     inv.DiscountCode,
            AppliedPackageId: inv.AppliedPackageId,
            PaymentMethod:    inv.PaymentMethod.ToString(),
            TransactionCode:  inv.TransactionCode,
            IssuedAt:         inv.IssuedAt,
            Items: inv.Items.Select(i => new InvoiceItemDto(
                i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice
            )).ToList()
        );
}
