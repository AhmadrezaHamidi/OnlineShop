using FluentValidation;
using Ahmad.OnlineShop.Application.Commands;

namespace Ahmad.OnlineShop.Application.Validators.Bnpl;

public sealed class IncreaseCreditLimitCommandValidator : AbstractValidator<IncreaseCreditLimitCommand>
{
    public IncreaseCreditLimitCommandValidator()
    {
        RuleFor(x => x.NewLimit)
            .GreaterThan(0).WithMessage("سقف اعتبار جدید باید بزرگ‌تر از صفر باشد.");
    }
}
