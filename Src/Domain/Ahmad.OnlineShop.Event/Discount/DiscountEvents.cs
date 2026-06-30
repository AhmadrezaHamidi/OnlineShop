using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Discount.Events;

/// <summary>تخفیف جدید ایجاد شد</summary>
public sealed record DiscountCreatedEvent(
    long    DiscountId,
    string  Code,
    int     Type,
    decimal Value
) : IEvent;

/// <summary>از کد تخفیف استفاده شد</summary>
public sealed record DiscountUsedEvent(
    long    DiscountId,
    string  Code,
    int     NewUsageCount,
    decimal DiscountAmount
) : IEvent;

/// <summary>پکیج محصول ایجاد شد</summary>
public sealed record PackageCreatedEvent(
    long    PackageId,
    string  Title,
    decimal DiscountPercent
) : IEvent;

/// <summary>پکیج فعال شد</summary>
public sealed record PackageActivatedEvent(
    long PackageId
) : IEvent;
