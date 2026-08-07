namespace Ahmad.OnlineShop.Domain.Discount.Strategies;

/// <summary>تخفیف درصدی — مبلغ * (درصد / 100)، گرد شده به تومان</summary>
public sealed class PercentageDiscountStrategy : IDiscountCalculationStrategy
{
    public decimal Calculate(decimal orderAmount, decimal discountValue)
        => Math.Round(orderAmount * discountValue / 100, 0);
}
