using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record BlockCreditCommand(
    long    UserId,
    decimal Amount
) : ICommand<long>;
