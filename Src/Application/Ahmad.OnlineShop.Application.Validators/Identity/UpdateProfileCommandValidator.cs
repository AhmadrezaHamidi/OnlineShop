using FluentValidation;
using Identity.Application.Commands;

namespace Ahmad.OnlineShop.Application.Validators.Identity;

public sealed class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("نام کامل الزامی است.")
            .MaximumLength(100).WithMessage("نام کامل نباید بیشتر از ۱۰۰ کاراکتر باشد.");
    }
}
