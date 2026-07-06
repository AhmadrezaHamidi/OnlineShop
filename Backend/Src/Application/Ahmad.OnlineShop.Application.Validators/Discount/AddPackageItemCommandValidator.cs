using FluentValidation;
using Ahmad.OnlineShop.Application.Commands.Discount;

namespace Ahmad.OnlineShop.Application.Validators.Discount;

public sealed class AddPackageItemCommandValidator : AbstractValidator<AddPackageItemCommand>
{
    public AddPackageItemCommandValidator()
    {
        RuleFor(x => x.PackageId)
            .GreaterThan(0).WithMessage("شناسه پکیج نامعتبر است.");

        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("شناسه محصول نامعتبر است.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("تعداد باید بزرگ‌تر از صفر باشد.");
    }
}
