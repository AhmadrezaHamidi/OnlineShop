using Microsoft.Extensions.Logging;

namespace Ahmad.OnlineShop.Application.EventHandlers.Bnpl;

/// <summary>
/// وقتی قرارداد به حالت معوق می‌رود (اقساط پرداخت نشده)
/// → هشدار به مدیر مالی + کاهش رتبه اعتباری کاربر
/// </summary>
public sealed class BnplContractDefaultedEventHandler(ILogger<BnplContractDefaultedEventHandler> logger)
    : IEventHandlerAsync<BnplContractDefaultedEvent>
{
    public Task HandleAsync(BnplContractDefaultedEvent @event, CancellationToken token)
    {
        logger.LogError(
            "قرارداد {ContractId} کاربر {UserId} معوق شد — نیاز به پیگیری مالی",
            @event.ContractId, @event.UserId);

        // TODO: ارسال هشدار به FinanceManager + کاهش credit score
        return Task.CompletedTask;
    }
}
