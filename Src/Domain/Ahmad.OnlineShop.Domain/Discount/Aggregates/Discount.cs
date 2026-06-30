using Ahmad.OnlineShop.Domain.Discount.Args;
using Ahmad.OnlineShop.Domain.Discount.Enums;
using Ahmad.OnlineShop.Domain.Discount.Events;
using Ahmad.OnlineShop.Domain.Discount.Exceptions;
using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Discount.Aggregates;

public sealed class Discount : AggregateRoot<long>
{
    public string        Code           { get; private set; } = string.Empty;
    public string        Title          { get; private set; } = string.Empty;
    public DiscountType  Type           { get; private set; }
    public decimal       Value          { get; private set; }
    public decimal?      MinOrderAmount { get; private set; }
    public int?          MaxUsage       { get; private set; }
    public int           UsageCount     { get; private set; }
    public DateTime?     ExpiresAt      { get; private set; }
    public bool          IsActive       { get; private set; }
    public DateTime      CreatedAt      { get; private set; }

    private Discount() { }

    private Discount(CreateDiscountArg arg) : base(arg.Id)
    {
        Code           = arg.Code.Trim().ToUpperInvariant();
        Title          = arg.Title.Trim();
        Type           = arg.Type;
        Value          = arg.Value;
        MinOrderAmount = arg.MinOrderAmount;
        MaxUsage       = arg.MaxUsage;
        UsageCount     = 0;
        ExpiresAt      = arg.ExpiresAt;
        IsActive       = true;
        CreatedAt      = DateTime.UtcNow;
    }

    public static Discount Create(CreateDiscountArg arg)
    {
        GuardCode(arg.Code);
        GuardValue(arg.Type, arg.Value);

        var discount = new Discount(arg);
        discount.RaiseDomainEvent(new DiscountCreatedEvent(arg.Id, arg.Code, (int)arg.Type, arg.Value));
        return discount;
    }

    // ── Apply ─────────────────────────────────────────────────────────────────

    /// <summary>مبلغ تخفیف را محاسبه و UsageCount را افزایش می‌دهد.</summary>
    public decimal Apply(decimal orderAmount)
    {
        GuardIsActive();
        GuardNotExpired();
        GuardUsageLimit();
        GuardMinOrderAmount(orderAmount);

        UsageCount++;

        var discountAmount = Type == DiscountType.Percentage
            ? Math.Round(orderAmount * Value / 100, 0)
            : Math.Min(Value, orderAmount);

        RaiseDomainEvent(new DiscountUsedEvent(Id, Code, UsageCount, discountAmount));
        return discountAmount;
    }

    // ── Status ────────────────────────────────────────────────────────────────

    public void Activate()
    {
        if (IsActive) throw new DiscountAlreadyActiveException();
        IsActive = true;
    }

    public void Deactivate()
    {
        if (!IsActive) throw new DiscountNotActiveException();
        IsActive = false;
    }

    // ── Guards ────────────────────────────────────────────────────────────────

    private static void GuardCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new InvalidDiscountCodeException();
    }

    private static void GuardValue(DiscountType type, decimal value)
    {
        if (value <= 0) throw new InvalidDiscountValueException();
        if (type == DiscountType.Percentage && value > 100)
            throw new InvalidDiscountValueException();
    }

    private void GuardIsActive()
    {
        if (!IsActive) throw new DiscountNotActiveException();
    }

    private void GuardNotExpired()
    {
        if (ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow)
            throw new DiscountExpiredException();
    }

    private void GuardUsageLimit()
    {
        if (MaxUsage.HasValue && UsageCount >= MaxUsage.Value)
            throw new DiscountMaxUsageReachedException();
    }

    private void GuardMinOrderAmount(decimal orderAmount)
    {
        if (MinOrderAmount.HasValue && orderAmount < MinOrderAmount.Value)
            throw new DiscountMinOrderAmountNotMetException();
    }
}
