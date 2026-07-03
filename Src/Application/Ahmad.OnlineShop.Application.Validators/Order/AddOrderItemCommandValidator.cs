using FluentValidation;
using Ahmad.OnlineShop.Application.Contract.Order.Commands;

namespace Ahmad.OnlineShop.Application.Validators.Order;

public sealed class AddOrderItemCommandValidator : AbstractValidator<AddOrderItemCommand>
{
    public AddOrderItemCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("شناسه محصول نامعتبر است.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("تعداد باید بزرگ‌تر از صفر باشد.");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("قیمت واحد باید بزرگ‌تر از صفر باشد.");
    }
}
