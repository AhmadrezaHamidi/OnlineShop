using DiscountAgg = Ahmad.OnlineShop.Domain.Discount.Aggregates.Discount;

namespace Ahmad.OnlineShop.Domain.Discount.Specifications;

public sealed class NotExpiredSpecification : IDiscountSpecification
{
    public string FailureReason => "کد تخفیف منقضی شده است";

    public bool IsSatisfiedBy(DiscountAgg discount, decimal orderAmount)
        => !discount.ExpiresAt.HasValue || discount.ExpiresAt.Value >= DateTime.UtcNow;
}
