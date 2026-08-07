using Ahmad.OnlineShop.Domain.Discount.Enums;

namespace Ahmad.OnlineShop.Domain.Discount.Strategies;

/// <summary>
/// Factory Pattern — بر اساس نوع تخفیف، strategy مناسب را برمی‌گرداند.
/// </summary>
public static class DiscountStrategyFactory
{
    public static IDiscountCalculationStrategy Create(DiscountType type) => type switch
    {
        DiscountType.Percentage  => new PercentageDiscountStrategy(),
        DiscountType.FixedAmount => new FixedAmountDiscountStrategy(),
        _                        => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}
