using FluentValidation;
using Ahmad.OnlineShop.Application.Commands;

namespace Ahmad.OnlineShop.Application.Validators.Product;

public sealed class ReplenishStockCommandValidator : AbstractValidator<ReplenishStockCommand>
{
    public ReplenishStockCommandValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("تعداد موجودی باید بزرگ‌تر از صفر باشد.");
    }
}
