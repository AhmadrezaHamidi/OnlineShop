using DiscountAgg = Ahmad.OnlineShop.Domain.Discount.Aggregates.Discount;

namespace Ahmad.OnlineShop.Domain.Discount.Specifications;

public interface IDiscountSpecification
{
    bool IsSatisfiedBy(DiscountAgg discount, decimal orderAmount);
    string FailureReason { get; }
}
