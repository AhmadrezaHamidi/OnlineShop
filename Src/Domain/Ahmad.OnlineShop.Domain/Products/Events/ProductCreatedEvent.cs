using Ahmad.OnlineShop.Domain.Products.Enums;
using AhmadBase.Doamin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmad.OnlineShop.Domain.Products.Events;

public sealed record ProductCreatedEvent(
    long    ProductId,
    long    SellerId,
    string  Name,
    decimal Price
) : IEvent;

/// <summary>قیمت محصول تغییر کرد</summary>
public sealed record ProductPriceChangedEvent(
    long ProductId,
    decimal OldPrice,
    decimal NewPrice
) : IEvent;

public sealed record ProductStatusChangedEvent(
    long ProductId,
    ProductStatus NewStatus
) : IEvent;

/// <summary>موجودی رزرو شد</summary>
public sealed record StockReservedEvent(
    long ProductId,
    int ReservedQuantity,
    int RemainingAvailable
) : IEvent;

/// <summary>موجودی آزاد شد (بعد از کنسل سفارش)</summary>
public sealed record StockReleasedEvent(
    long ProductId,
    int ReleasedQuantity
) : IEvent;

/// <summary>موجودی به پایان رسید یا زیر حد بحرانی رفت</summary>
public sealed record StockDepletedEvent(
    long ProductId,
    int CurrentQuantity
) : IEvent;

/// <summary>موجودی اضافه شد</summary>
public sealed record StockReplenishedEvent(
    long ProductId,
    int AddedQuantity,
    int NewTotalQuantity
) : IEvent;

/// <summary>تصویر جدیدی به محصول اضافه شد</summary>
public sealed record ProductImageAddedEvent(
    long ProductId,
    Guid ImageId,
    string Url,
    ImageType Type
) : IEvent;

/// <summary>تصویر از محصول حذف شد</summary>
public sealed record ProductImageRemovedEvent(
    long ProductId,
    Guid ImageId
) : IEvent;
