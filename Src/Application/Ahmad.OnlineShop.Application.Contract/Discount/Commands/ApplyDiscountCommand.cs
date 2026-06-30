namespace Ahmad.OnlineShop.Application.Commands.Discount;

/// <summary>اعمال کد تخفیف روی مبلغ سفارش — مبلغ تخفیف را برمی‌گرداند</summary>
public sealed record ApplyDiscountCommand(
    string  Code,
    decimal OrderAmount
) : ICommand<decimal>;
