using FluentValidation;
using Ahmad.OnlineShop.Application.Commands;

namespace Ahmad.OnlineShop.Application.Validators.Product;

public sealed class AddProductImageCommandValidator : AbstractValidator<AddProductImageCommand>
{
    public AddProductImageCommandValidator()
    {
        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("آدرس تصویر الزامی است.")
            .MaximumLength(500).WithMessage("آدرس تصویر نباید بیشتر از ۵۰۰ کاراکتر باشد.");

        RuleFor(x => x.BucketKey)
            .NotEmpty().WithMessage("کلید bucket الزامی است.")
            .MaximumLength(200).WithMessage("کلید bucket نباید بیشتر از ۲۰۰ کاراکتر باشد.");
    }
}
