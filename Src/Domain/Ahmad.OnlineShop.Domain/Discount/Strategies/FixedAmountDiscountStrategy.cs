namespace Ahmad.OnlineShop.Domain.Discount.Strategies;

/// <summary>تخفیف مبلغ ثابت — نمی‌تواند از کل سفارش بیشتر باشد</summary>
public sealed class FixedAmountDiscountStrategy : IDiscountCalculationStrategy
{
    public decimal Calculate(decimal orderAmount, decimal discountValue)
        => Math.Min(discountValue, orderAmount);
}
