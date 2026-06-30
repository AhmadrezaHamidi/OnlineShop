using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Products.Events;

// ── Product ───────────────────────────────────────────────────────────────────

/// <summary>محصول جدید ایجاد شد</summary>
public sealed record ProductCreatedEvent(
    long    ProductId,
    long    SellerId,
    string  Name,
    decimal Price
) : IEvent;

/// <summary>قیمت محصول تغییر کرد</summary>
public sealed record ProductPriceChangedEvent(
    long    ProductId,
    decimal OldPrice,
    decimal NewPrice
) : IEvent;

/// <summary>وضعیت محصول تغییر کرد</summary>
public sealed record ProductStatusChangedEvent(
    long ProductId,
    int  NewStatus   // ProductStatus as int
) : IEvent;

// ── Inventory ─────────────────────────────────────────────────────────────────

/// <summary>موجودی رزرو شد</summary>
public sealed record StockReservedEvent(
    long ProductId,
    int  ReservedQuantity,
    int  RemainingAvailable
) : IEvent;

/// <summary>موجودی آزاد شد (بعد از کنسل سفارش)</summary>
public sealed record StockReleasedEvent(
    long ProductId,
    int  ReleasedQuantity
) : IEvent;

/// <summary>موجودی به پایان رسید یا زیر حد بحرانی رفت</summary>
public sealed record StockDepletedEvent(
    long ProductId,
    int  CurrentQuantity
) : IEvent;

/// <summary>موجودی تأمین شد</summary>
public sealed record StockReplenishedEvent(
    long ProductId,
    int  AddedQuantity,
    int  NewTotalQuantity
) : IEvent;

// ── Images ────────────────────────────────────────────────────────────────────

/// <summary>تصویر جدیدی به محصول اضافه شد</summary>
public sealed record ProductImageAddedEvent(
    long   ProductId,
    Guid   ImageId,
    string Url,
    int    Type       // ImageType as int
) : IEvent;

/// <summary>تصویر از محصول حذف شد</summary>
public sealed record ProductImageRemovedEvent(
    long ProductId,
    Guid ImageId
) : IEvent;
