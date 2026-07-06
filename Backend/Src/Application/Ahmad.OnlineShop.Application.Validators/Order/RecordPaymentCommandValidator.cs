using FluentValidation;
using Ahmad.OnlineShop.Application.Contract.Order.Commands;

namespace Ahmad.OnlineShop.Application.Validators.Order;

public sealed class RecordPaymentCommandValidator : AbstractValidator<RecordPaymentCommand>
{
    public RecordPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .GreaterThan(0).WithMessage("شناسه پرداخت نامعتبر است.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("مبلغ پرداخت باید بزرگ‌تر از صفر باشد.");

        RuleFor(x => x.Provider)
            .MaximumLength(50).WithMessage("نام درگاه پرداخت نباید بیشتر از ۵۰ کاراکتر باشد.")
            .When(x => x.Provider is not null);
    }
}
