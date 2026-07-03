using FluentValidation;
using Ahmad.OnlineShop.Application.Commands.Discount;

namespace Ahmad.OnlineShop.Application.Validators.Discount;

public sealed class CreatePackageCommandValidator : AbstractValidator<CreatePackageCommand>
{
    public CreatePackageCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("عنوان پکیج الزامی است.")
            .MaximumLength(200).WithMessage("عنوان پکیج نباید بیشتر از ۲۰۰ کاراکتر باشد.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("توضیحات پکیج نباید بیشتر از ۱۰۰۰ کاراکتر باشد.")
            .When(x => x.Description is not null);

        RuleFor(x => x.DiscountPercent)
            .GreaterThan(0).WithMessage("درصد تخفیف باید بزرگ‌تر از صفر باشد.")
            .LessThanOrEqualTo(100).WithMessage("درصد تخفیف نباید بیشتر از ۱۰۰ باشد.");

        RuleFor(x => x.ValidFrom)
            .LessThan(x => x.ValidTo).WithMessage("تاریخ شروع باید قبل از تاریخ پایان باشد.");

        RuleFor(x => x.ValidTo)
            .GreaterThan(DateTime.UtcNow).WithMessage("تاریخ پایان باید در آینده باشد.");
    }
}
