namespace Ahmad.OnlineShop.Domain.Discount.Strategies;

/// <summary>
/// Strategy Pattern — هر نوع تخفیف الگوریتم محاسبه خودش را دارد.
/// اضافه کردن نوع جدید = فقط یک کلاس جدید، بدون تغییر Discount aggregate.
/// </summary>
public interface IDiscountCalculationStrategy
{
    decimal Calculate(decimal orderAmount, decimal discountValue);
}
