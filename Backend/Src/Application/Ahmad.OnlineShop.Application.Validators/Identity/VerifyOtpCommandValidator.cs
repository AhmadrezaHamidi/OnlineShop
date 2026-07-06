using FluentValidation;
using Identity.Application.Commands;

namespace Ahmad.OnlineShop.Application.Validators.Identity;

public sealed class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("شماره موبایل الزامی است.")
            .Matches(@"^09[0-9]{9}$").WithMessage("شماره موبایل باید ۱۱ رقم و با ۰۹ شروع شود.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("کد OTP الزامی است.")
            .Length(6).WithMessage("کد OTP باید دقیقاً ۶ رقم باشد.")
            .Matches(@"^\d{6}$").WithMessage("کد OTP فقط شامل اعداد است.");
    }
}
