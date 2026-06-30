using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Order.Events;

// ── Value Object ─────────────────────────────────────────────────────────────

/// <summary>اطلاعات آیتم سفارش در event</summary>
public sealed record OrderItemSnapshot(
    long    ProductId,
    int     Quantity,
    decimal UnitPrice
);

// ── Order Lifecycle ──────────────────────────────────────────────────────────

/// <summary>سفارش جدید ایجاد شد</summary>
public sealed record OrderCreatedEvent(
    long   OrderId,
    long   UserId,
    int    PaymentMethod    // PaymentMethod as int
) : IEvent;

/// <summary>سفارش ثبت نهایی شد</summary>
public sealed record OrderPlacedEvent(
    long                             OrderId,
    long                             UserId,
    decimal                          TotalAmount,
    int                              PaymentMethod,    // PaymentMethod as int
    IReadOnlyList<OrderItemSnapshot> Items
) : IEvent;

/// <summary>سفارش پس از پرداخت موفق تأیید شد</summary>
public sealed record OrderConfirmedEvent(
    long    OrderId,
    long    UserId,
    decimal TotalAmount
) : IEvent;

/// <summary>سفارش ارسال شد</summary>
public sealed record OrderShippedEvent(
    long OrderId,
    long UserId
) : IEvent;

/// <summary>سفارش تحویل داده شد</summary>
public sealed record OrderDeliveredEvent(
    long OrderId,
    long UserId
) : IEvent;

/// <summary>سفارش لغو شد — موجودی باید آزاد بشه</summary>
public sealed record OrderCancelledEvent(
    long                             OrderId,
    long                             UserId,
    string                           Reason,
    IReadOnlyList<OrderItemSnapshot> Items
) : IEvent;

// ── Order Items ──────────────────────────────────────────────────────────────

/// <summary>آیتم به سفارش افزوده شد — موجودی Reserve بشه</summary>
public sealed record OrderItemAddedEvent(
    long    OrderId,
    long    ItemId,
    long    ProductId,
    int     Quantity,
    decimal UnitPrice,
    decimal NewTotalAmount
) : IEvent;

/// <summary>آیتم از سفارش حذف شد — موجودی Release بشه</summary>
public sealed record OrderItemRemovedEvent(
    long    OrderId,
    long    ItemId,
    long    ProductId,
    int     Quantity,
    decimal NewTotalAmount
) : IEvent;

// ── Payment ──────────────────────────────────────────────────────────────────

/// <summary>پرداخت ثبت شد</summary>
public sealed record PaymentRecordedEvent(
    long    PaymentId,
    long    OrderId,
    decimal Amount,
    int     Status,         // PaymentStatus as int
    string? Provider
) : IEvent;
