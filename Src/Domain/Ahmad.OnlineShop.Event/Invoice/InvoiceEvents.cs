using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Invoice.Events;

/// <summary>فاکتور صادر شد — می‌توان برای ارسال ایمیل/نوتیفیکیشن استفاده کرد</summary>
public sealed record InvoiceIssuedEvent(
    long    InvoiceId,
    long    OrderId,
    long    UserId,
    decimal TotalAmount,
    string  InvoiceNumber
) : IEvent;

/// <summary>فاکتور باطل شد (لغو سفارش)</summary>
public sealed record InvoiceVoidedEvent(
    long InvoiceId,
    long OrderId
) : IEvent;
