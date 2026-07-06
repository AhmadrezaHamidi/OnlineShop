using FluentValidation;
using Ahmad.OnlineShop.Application.Commands;

namespace Ahmad.OnlineShop.Application.Validators.Bnpl;

public sealed class CreateBnplContractCommandValidator : AbstractValidator<CreateBnplContractCommand>
{
    public CreateBnplContractCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0).WithMessage("شناسه سفارش نامعتبر است.");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("شناسه کاربر نامعتبر است.");

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0).WithMessage("مبلغ کل باید بزرگ‌تر از صفر باشد.");

        RuleFor(x => x.InstallmentCount)
            .GreaterThanOrEqualTo(1).WithMessage("تعداد اقساط باید حداقل ۱ باشد.")
            .LessThanOrEqualTo(60).WithMessage("تعداد اقساط نباید بیشتر از ۶۰ باشد.");

        RuleFor(x => x.FirstDueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("تاریخ اولین قسط باید در آینده باشد.");

        RuleFor(x => x.IntervalDays)
            .GreaterThanOrEqualTo(1).WithMessage("فاصله بین اقساط باید حداقل ۱ روز باشد.");
    }
}
