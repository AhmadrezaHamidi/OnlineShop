using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Invoice.Entities;

public sealed class InvoiceItem : TEntity<long>
{
    public long    InvoiceId   { get; private set; }
    public long    ProductId   { get; private set; }
    public string  ProductName { get; private set; } = string.Empty;
    public int     Quantity    { get; private set; }
    public decimal UnitPrice   { get; private set; }
    public decimal TotalPrice  => Quantity * UnitPrice;

    private InvoiceItem() { }

    internal InvoiceItem(long id, long invoiceId, long productId, string productName,
                          int quantity, decimal unitPrice)
    {
        Id          = id;
        InvoiceId   = invoiceId;
        ProductId   = productId;
        ProductName = productName;
        Quantity    = quantity;
        UnitPrice   = unitPrice;
    }
}
