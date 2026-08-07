namespace Ahmad.OnlineShop.Domain.Discount.Specifications;

/// <summary>
/// تمام قوانین اعتبارسنجی تخفیف را به صورت یک زنجیر AND ترکیب می‌کند.
/// </summary>
public static class DiscountSpecificationBuilder
{
    public static IDiscountSpecification BuildFullValidation()
        => new IsActiveSpecification()
            .And(new NotExpiredSpecification())
            .And(new UsageLimitSpecification())
            .And(new MinOrderAmountSpecification());
}
