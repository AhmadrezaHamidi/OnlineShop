using DiscountAgg = Ahmad.OnlineShop.Domain.Discount.Aggregates.Discount;

namespace Ahmad.OnlineShop.Domain.Discount.Specifications;

public sealed class UsageLimitSpecification : IDiscountSpecification
{
    public string FailureReason => "سقف استفاده از این کد تخفیف پر شده است";

    public bool IsSatisfiedBy(DiscountAgg discount, decimal orderAmount)
        => !discount.MaxUsage.HasValue || discount.UsageCount < discount.MaxUsage.Value;
}
