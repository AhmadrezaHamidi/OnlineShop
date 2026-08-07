using DiscountAgg = Ahmad.OnlineShop.Domain.Discount.Aggregates.Discount;

namespace Ahmad.OnlineShop.Domain.Discount.Specifications;

public sealed class IsActiveSpecification : IDiscountSpecification
{
    public string FailureReason => "کد تخفیف غیرفعال است";

    public bool IsSatisfiedBy(DiscountAgg discount, decimal orderAmount)
        => discount.IsActive;
}
