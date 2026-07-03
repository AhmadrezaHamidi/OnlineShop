using FluentValidation;
using Ahmad.OnlineShop.Application.Commands;

namespace Ahmad.OnlineShop.Application.Validators.Product;

public sealed class ChangeProductPriceCommandValidator : AbstractValidator<ChangeProductPriceCommand>
{
    public ChangeProductPriceCommandValidator()
    {
        RuleFor(x => x.NewPrice)
            .GreaterThan(0).WithMessage("قیمت جدید باید بزرگ‌تر از صفر باشد.");
    }
}
