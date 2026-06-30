namespace Ahmad.OnlineShop.Application.Commands.Marketing;

/// <summary>ارسال پیامک دسته‌جمعی به همه مشتریان</summary>
public sealed record SendBulkSmsCommand(
    string Message
) : ICommand<int>; // تعداد پیامک‌های ارسال‌شده
