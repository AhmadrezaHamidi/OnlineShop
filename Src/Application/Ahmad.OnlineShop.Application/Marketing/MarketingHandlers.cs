using Ahmad.OnlineShop.Application.Commands.Marketing;
using Identity.Application.Query.Contracts;

namespace Ahmad.OnlineShop.Application.Handlers.Marketing;

/// <summary>
/// Handler بازاریابی — ارسال پیامک دسته‌جمعی به همه مشتریان
/// </summary>
public sealed class MarketingHandlers(
    IUserReadRepository userRepo,
    ISmsService         smsService) :
    ICommandHandler<SendBulkSmsCommand, int>
{
    public async Task<int> Handle(SendBulkSmsCommand cmd, CancellationToken token)
    {
        var phones = await userRepo.GetAllCustomerPhonesAsync(token);
        if (phones.Count == 0) return 0;

        await smsService.SendBulkAsync(phones, cmd.Message, token);
        return phones.Count;
    }
}
