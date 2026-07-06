using FluentValidation;
using Ahmad.OnlineShop.Application.Contract.Order.Commands;

namespace Ahmad.OnlineShop.Application.Validators.Order;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("شناسه کاربر نامعتبر است.");
    }
}
