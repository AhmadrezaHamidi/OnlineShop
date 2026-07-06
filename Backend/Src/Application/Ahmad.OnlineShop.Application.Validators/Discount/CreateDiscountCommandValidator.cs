using FluentValidation;
using Ahmad.OnlineShop.Application.Commands.Discount;

namespace Ahmad.OnlineShop.Application.Validators.Discount;

public sealed class CreateDiscountCommandValidator : AbstractValidator<CreateDiscountCommand>
{
    public CreateDiscountCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("کد تخفیف الزامی است.")
            .MaximumLength(50).WithMessage("کد تخفیف نباید بیشتر از ۵۰ کاراکتر باشد.")
            .Matches(@"^[A-Za-z0-9\-_]+$").WithMessage("کد تخفیف فقط می‌تواند شامل حروف، اعداد، خط تیره و زیرخط باشد.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("عنوان تخفیف الزامی است.")
            .MaximumLength(200).WithMessage("عنوان تخفیف نباید بیشتر از ۲۰۰ کاراکتر باشد.");

        RuleFor(x => x.Value)
            .GreaterThan(0).WithMessage("مقدار تخفیف باید بزرگ‌تر از صفر باشد.");

        RuleFor(x => x.MinOrderAmount)
            .GreaterThan(0).WithMessage("حداقل مبلغ سفارش باید بزرگ‌تر از صفر باشد.")
            .When(x => x.MinOrderAmount.HasValue);

        RuleFor(x => x.MaxUsage)
            .GreaterThan(0).WithMessage("حداکثر تعداد استفاده باید بزرگ‌تر از صفر باشد.")
            .When(x => x.MaxUsage.HasValue);

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTime.UtcNow).WithMessage("تاریخ انقضا باید در آینده باشد.")
            .When(x => x.ExpiresAt.HasValue);
    }
}
