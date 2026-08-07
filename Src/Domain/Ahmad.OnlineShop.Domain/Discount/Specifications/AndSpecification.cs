using DiscountAgg = Ahmad.OnlineShop.Domain.Discount.Aggregates.Discount;

namespace Ahmad.OnlineShop.Domain.Discount.Specifications;

/// <summary>ترکیب دو specification با منطق AND — اگر هر کدام fail کنند، کل fail است.</summary>
public sealed class AndSpecification : IDiscountSpecification
{
    private readonly IDiscountSpecification _left;
    private readonly IDiscountSpecification _right;

    public AndSpecification(IDiscountSpecification left, IDiscountSpecification right)
    {
        _left  = left;
        _right = right;
    }

    public string FailureReason { get; private set; } = string.Empty;

    public bool IsSatisfiedBy(DiscountAgg discount, decimal orderAmount)
    {
        if (!_left.IsSatisfiedBy(discount, orderAmount))
        {
            FailureReason = _left.FailureReason;
            return false;
        }

        if (!_right.IsSatisfiedBy(discount, orderAmount))
        {
            FailureReason = _right.FailureReason;
            return false;
        }

        return true;
    }
}

public static class DiscountSpecificationExtensions
{
    public static IDiscountSpecification And(
        this IDiscountSpecification left,
        IDiscountSpecification right)
        => new AndSpecification(left, right);
}
