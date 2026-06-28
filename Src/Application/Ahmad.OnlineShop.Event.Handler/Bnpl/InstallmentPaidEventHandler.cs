using Microsoft.Extensions.Logging;

namespace Ahmad.OnlineShop.Application.EventHandlers.Bnpl;

/// <summary>
/// وقتی یک قسط پرداخت می‌شود
/// → رهاسازی اعتبار متناسب با مبلغ قسط
/// </summary>
public sealed class InstallmentPaidEventHandler(
    Ahmad.OnlineShop.Domain.Repositories.ICreditLimitRepository creditRepo,
    ILogger<InstallmentPaidEventHandler>                        logger)
    : IEventHandlerAsync<InstallmentPaidEvent>
{
    public async Task HandleAsync(InstallmentPaidEvent @event, CancellationToken token)
    {
        var credit = await creditRepo.GetByUserIdAsync(@event.UserId, token);
        if (credit is null) return;

        // آزادسازی اعتبار معادل مبلغ قسط پرداخت‌شده
        credit.Release(@event.Amount);
        await creditRepo.UpdateAsync(credit, token);

        logger.LogInformation(
            "قسط {No}/{ContractId} به مبلغ {Amount} پرداخت شد — {Available} اعتبار آزاد",
            @event.InstallmentNo, @event.ContractId, @event.Amount, credit.AvailableLimit);
    }
}
