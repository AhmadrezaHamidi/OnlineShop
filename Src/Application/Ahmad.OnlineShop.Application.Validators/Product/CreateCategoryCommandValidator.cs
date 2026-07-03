using FluentValidation;
using Ahmad.OnlineShop.Application.Commands;

namespace Ahmad.OnlineShop.Application.Validators.Product;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام دسته‌بندی الزامی است.")
            .MaximumLength(100).WithMessage("نام دسته‌بندی نباید بیشتر از ۱۰۰ کاراکتر باشد.");

        RuleFor(x => x.ParentId)
            .GreaterThan(0).WithMessage("شناسه دسته‌بندی والد نامعتبر است.")
            .When(x => x.ParentId.HasValue);
    }
}
