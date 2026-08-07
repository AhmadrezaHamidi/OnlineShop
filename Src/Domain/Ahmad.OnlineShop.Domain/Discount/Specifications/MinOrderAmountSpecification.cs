using DiscountAgg = Ahmad.OnlineShop.Domain.Discount.Aggregates.Discount;

namespace Ahmad.OnlineShop.Domain.Discount.Specifications;

public sealed class MinOrderAmountSpecification : IDiscountSpecification
{
    public string FailureReason => "مبلغ سفارش از حداقل مبلغ مجاز برای این تخفیف کمتر است";

    public bool IsSatisfiedBy(DiscountAgg discount, decimal orderAmount)
        => !discount.MinOrderAmount.HasValue || orderAmount >= discount.MinOrderAmount.Value;
}
