using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Contract.Order.Commands;

/// <summary>اعمال پکیج تخفیفی به سفارش</summary>
public record ApplyPackageToOrderCommand(
    long OrderId,
    long PackageId
) : ICommand<decimal>;
