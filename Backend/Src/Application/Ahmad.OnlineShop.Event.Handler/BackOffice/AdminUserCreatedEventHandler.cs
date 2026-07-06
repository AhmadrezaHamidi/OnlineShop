using Microsoft.Extensions.Logging;

namespace Ahmad.OnlineShop.Application.EventHandlers.BackOffice;

/// <summary>
/// وقتی ادمین جدید ایجاد می‌شود
/// → لاگ امنیتی + اطلاع به SuperAdmin
/// </summary>
public sealed class AdminUserCreatedEventHandler(ILogger<AdminUserCreatedEventHandler> logger)
    : IEventHandlerAsync<AdminUserCreatedEvent>
{
    public Task HandleAsync(AdminUserCreatedEvent @event, CancellationToken token)
    {
        logger.LogWarning(
            "[SECURITY] ادمین جدید ایجاد شد — Id:{Id} | نام:{Name} | ایمیل:{Email} | نقش:{Role}",
            @event.AdminUserId, @event.FullName, @event.Email, @event.Role);

        // TODO: ارسال ایمیل خوش‌آمدگویی + اطلاع به SuperAdmin
        return Task.CompletedTask;
    }
}
