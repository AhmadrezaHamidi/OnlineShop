using FluentValidation;
using Ahmad.OnlineShop.Application.Commands.Marketing;

namespace Ahmad.OnlineShop.Application.Validators.Marketing;

public sealed class SendBulkSmsCommandValidator : AbstractValidator<SendBulkSmsCommand>
{
    public SendBulkSmsCommandValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("متن پیامک الزامی است.")
            .MaximumLength(500).WithMessage("متن پیامک نباید بیشتر از ۵۰۰ کاراکتر باشد.");
    }
}
