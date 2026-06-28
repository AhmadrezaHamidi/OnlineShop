using Ahmad.OnlineShop.Domain.Bnpl.Aggregates;
using Ahmad.OnlineShop.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Ahmad.OnlineShop.Application.EventHandlers.Bnpl;

/// <summary>
/// وقتی قرارداد اقساطی لغو می‌شود
/// → اعتبار مسدودشده آزاد می‌شود
/// </summary>
public sealed class BnplContractCancelledEventHandler(
    ICreditLimitRepository              creditRepo,
    ILogger<BnplContractCancelledEventHandler> logger)
    : IEventHandlerAsync<BnplContractCancelledEvent>
{
    public async Task HandleAsync(BnplContractCancelledEvent @event, CancellationToken token)
    {
        var credit = await creditRepo.GetByUserIdAsync(@event.UserId, token);
        if (credit is null)
        {
            logger.LogWarning("اعتبار کاربر {UserId} برای آزادسازی پیدا نشد", @event.UserId);
            return;
        }

        credit.Release(@event.ReleasedAmount);
        await creditRepo.UpdateAsync(credit, token);

        logger.LogInformation(
            "قرارداد {ContractId} لغو شد — {Amount} اعتبار برای کاربر {UserId} آزاد شد",
            @event.ContractId, @event.ReleasedAmount, @event.UserId);
    }
}
