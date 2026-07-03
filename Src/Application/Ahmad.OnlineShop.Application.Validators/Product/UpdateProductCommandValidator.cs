using FluentValidation;
using Ahmad.OnlineShop.Application.Commands;

namespace Ahmad.OnlineShop.Application.Validators.Product;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام محصول الزامی است.")
            .MaximumLength(200).WithMessage("نام محصول نباید بیشتر از ۲۰۰ کاراکتر باشد.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("توضیحات نباید بیشتر از ۲۰۰۰ کاراکتر باشد.")
            .When(x => x.Description is not null);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("شناسه دسته‌بندی نامعتبر است.");
    }
}
