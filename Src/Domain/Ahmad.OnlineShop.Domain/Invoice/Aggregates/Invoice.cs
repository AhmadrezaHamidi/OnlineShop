using Ahmad.OnlineShop.Domain.Invoice.Entities;
using Ahmad.OnlineShop.Domain.Invoice.Enums;
using Ahmad.OnlineShop.Domain.Invoice.Events;
using Ahmad.OnlineShop.Domain.Order.Enums;
using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Invoice.Aggregates;

/// <summary>
/// فاکتور/رسید — با استفاده از Observer Pattern (Domain Event) به صورت خودکار
/// پس از تأیید سفارش (OrderConfirmedEvent) ایجاد می‌شود.
/// </summary>
public sealed class Invoice : AggregateRoot<long>
{
    private readonly List<InvoiceItem> _items = [];

    public long          OrderId             { get; private set; }
    public long          UserId              { get; private set; }
    public string        InvoiceNumber       { get; private set; } = string.Empty;
    public InvoiceStatus Status              { get; private set; }
    public decimal       SubTotal            { get; private set; }
    public decimal       DiscountAmount      { get; private set; }
    public decimal       PackageDiscount     { get; private set; }
    public decimal       TotalAmount         { get; private set; }
    public string?       DiscountCode        { get; private set; }
    public long?         AppliedPackageId    { get; private set; }
    public PaymentMethod PaymentMethod       { get; private set; }
    public string?       TransactionCode     { get; private set; }
    public DateTime      IssuedAt            { get; private set; }

    public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();

    private Invoice() { }

    public static Invoice Create(CreateInvoiceArg arg)
    {
        var inv = new Invoice();
        inv.Id              = arg.Id;
        inv.OrderId         = arg.OrderId;
        inv.UserId          = arg.UserId;
        inv.InvoiceNumber   = GenerateNumber(arg.Id);
        inv.Status          = InvoiceStatus.Issued;
        inv.SubTotal        = arg.SubTotal;
        inv.DiscountAmount  = arg.DiscountAmount;
        inv.PackageDiscount = arg.PackageDiscount;
        inv.TotalAmount     = arg.TotalAmount;
        inv.DiscountCode    = arg.DiscountCode;
        inv.AppliedPackageId= arg.AppliedPackageId;
        inv.PaymentMethod   = arg.PaymentMethod;
        inv.TransactionCode = arg.TransactionCode;
        inv.IssuedAt        = DateTime.UtcNow;

        foreach (var item in arg.Items)
            inv._items.Add(new InvoiceItem(item.ItemId, arg.Id,
                item.ProductId, item.ProductName, item.Quantity, item.UnitPrice));

        inv.RaiseDomainEvent(new InvoiceIssuedEvent(arg.Id, arg.OrderId, arg.UserId,
            arg.TotalAmount, inv.InvoiceNumber));

        return inv;
    }

    public void Void()
    {
        if (Status != InvoiceStatus.Issued)
            throw new InvalidOperationException("فقط فاکتورهای صادرشده قابل ابطال هستند");
        Status = InvoiceStatus.Voided;
    }

    public void Refund()
    {
        if (Status != InvoiceStatus.Issued)
            throw new InvalidOperationException("فقط فاکتورهای صادرشده قابل استرداد هستند");
        Status = InvoiceStatus.Refunded;
    }

    private static string GenerateNumber(long id)
        => $"INV-{DateTime.UtcNow:yyyyMM}-{id:D6}";
}
