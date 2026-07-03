using FluentValidation;
using Ahmad.OnlineShop.Application.Commands.Discount;

namespace Ahmad.OnlineShop.Application.Validators.Discount;

public sealed class ApplyDiscountCommandValidator : AbstractValidator<ApplyDiscountCommand>
{
    public ApplyDiscountCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("کد تخفیف الزامی است.")
            .MaximumLength(50).WithMessage("کد تخفیف نباید بیشتر از ۵۰ کاراکتر باشد.");

        RuleFor(x => x.OrderAmount)
            .GreaterThan(0).WithMessage("مبلغ سفارش باید بزرگ‌تر از صفر باشد.");
    }
}
