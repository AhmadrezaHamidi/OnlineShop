using FluentValidation;
using Identity.Application.Commands;

namespace Ahmad.OnlineShop.Application.Validators.Identity;

public sealed class RequestOtpCommandValidator : AbstractValidator<RequestOtpCommand>
{
    public RequestOtpCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("شماره موبایل الزامی است.")
            .Matches(@"^09[0-9]{9}$").WithMessage("شماره موبایل باید ۱۱ رقم و با ۰۹ شروع شود.");
    }
}
