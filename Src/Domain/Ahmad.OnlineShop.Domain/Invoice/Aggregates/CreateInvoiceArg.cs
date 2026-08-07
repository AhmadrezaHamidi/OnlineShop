using Ahmad.OnlineShop.Domain.Order.Enums;

namespace Ahmad.OnlineShop.Domain.Invoice.Aggregates;

public sealed record CreateInvoiceArg(
    long                    Id,
    long                    OrderId,
    long                    UserId,
    decimal                 SubTotal,
    decimal                 DiscountAmount,
    decimal                 PackageDiscount,
    decimal                 TotalAmount,
    string?                 DiscountCode,
    long?                   AppliedPackageId,
    PaymentMethod           PaymentMethod,
    string?                 TransactionCode,
    IReadOnlyList<InvoiceItemArg> Items
);

public sealed record InvoiceItemArg(
    long    ItemId,
    long    ProductId,
    string  ProductName,
    int     Quantity,
    decimal UnitPrice
);
