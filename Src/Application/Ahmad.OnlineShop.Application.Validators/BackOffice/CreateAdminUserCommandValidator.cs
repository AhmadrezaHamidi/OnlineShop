using FluentValidation;
using BackOffice.Application.Commands;

namespace Ahmad.OnlineShop.Application.Validators.BackOffice;

public sealed class CreateAdminUserCommandValidator : AbstractValidator<CreateAdminUserCommand>
{
    public CreateAdminUserCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("نام کامل الزامی است.")
            .MaximumLength(100).WithMessage("نام کامل نباید بیشتر از ۱۰۰ کاراکتر باشد.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("ایمیل الزامی است.")
            .EmailAddress().WithMessage("فرمت ایمیل نامعتبر است.")
            .MaximumLength(200).WithMessage("ایمیل نباید بیشتر از ۲۰۰ کاراکتر باشد.");
    }
}
