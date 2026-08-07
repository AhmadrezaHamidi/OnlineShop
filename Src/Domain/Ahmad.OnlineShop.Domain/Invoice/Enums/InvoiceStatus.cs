namespace Ahmad.OnlineShop.Domain.Invoice.Enums;

public enum InvoiceStatus
{
    Issued   = 1,  // صادر شده
    Voided   = 2,  // باطل شده (سفارش لغو)
    Refunded = 3   // مسترد شده
}
