using FluentValidation;
using Ahmad.OnlineShop.Application.Commands;

namespace Ahmad.OnlineShop.Application.Validators.Bnpl;

public sealed class PayInstallmentCommandValidator : AbstractValidator<PayInstallmentCommand>
{
    public PayInstallmentCommandValidator()
    {
        RuleFor(x => x.ContractId)
            .GreaterThan(0).WithMessage("شناسه قرارداد نامعتبر است.");

        RuleFor(x => x.InstallmentId)
            .GreaterThan(0).WithMessage("شناسه قسط نامعتبر است.");
    }
}
