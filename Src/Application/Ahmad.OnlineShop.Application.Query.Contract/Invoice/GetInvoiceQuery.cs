using AhmadBase.Application.Query;
using Ahmad.OnlineShop.Domain.Invoice.Enums;
using Ahmad.OnlineShop.Domain.Order.Enums;

namespace Ahmad.OnlineShop.Application.Query.Contract.Invoice;

// ── Queries ──────────────────────────────────────────────────────────────────

public record GetInvoiceByIdQuery(long InvoiceId) : IQuery<GetInvoiceResponse?>;

public record GetInvoiceByOrderIdQuery(long OrderId) : IQuery<GetInvoiceResponse?>;

public record GetMyInvoicesQuery(long UserId, int Page = 1, int PageSize = 20)
    : IQuery<PagedResult<GetInvoiceResponse>>;

// ── Responses ─────────────────────────────────────────────────────────────────

public record GetInvoiceResponse(
    long                          Id,
    long                          OrderId,
    long                          UserId,
    string                        InvoiceNumber,
    InvoiceStatus                 Status,
    decimal                       SubTotal,
    decimal                       DiscountAmount,
    decimal                       PackageDiscount,
    decimal                       TotalAmount,
    string?                       DiscountCode,
    long?                         AppliedPackageId,
    string                        PaymentMethod,
    string?                       TransactionCode,
    DateTime                      IssuedAt,
    IReadOnlyList<InvoiceItemDto> Items
);

public record InvoiceItemDto(
    long    ProductId,
    string  ProductName,
    int     Quantity,
    decimal UnitPrice,
    decimal TotalPrice
);
