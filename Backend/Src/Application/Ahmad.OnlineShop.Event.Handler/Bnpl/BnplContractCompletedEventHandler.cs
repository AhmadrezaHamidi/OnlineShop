using Microsoft.Extensions.Logging;

namespace Ahmad.OnlineShop.Application.EventHandlers.Bnpl;

/// <summary>
/// وقتی قرارداد اقساطی کامل می‌شود (همه اقساط پرداخت)
/// → پیام تبریک به کاربر و بهبود رتبه اعتباری
/// </summary>
public sealed class BnplContractCompletedEventHandler(ILogger<BnplContractCompletedEventHandler> logger)
    : IEventHandlerAsync<BnplContractCompletedEvent>
{
    public Task HandleAsync(BnplContractCompletedEvent @event, CancellationToken token)
    {
        logger.LogInformation(
            "قرارداد {ContractId} کاربر {UserId} با موفقیت تکمیل شد",
            @event.ContractId, @event.UserId);

        // TODO: بهبود رتبه اعتباری کاربر + ارسال SMS تبریک
        return Task.CompletedTask;
    }
}
