using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Contract.Order.Commands;

/// <summary>اعمال کد تخفیف به سفارش و ثبت مبلغ کاهش‌یافته</summary>
public record ApplyDiscountToOrderCommand(
    long   OrderId,
    string DiscountCode
) : ICommand<decimal>;
