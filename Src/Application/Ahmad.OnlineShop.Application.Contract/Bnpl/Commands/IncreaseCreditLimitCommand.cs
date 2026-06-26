using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record IncreaseCreditLimitCommand(
    long    UserId,
    decimal NewLimit
) : ICommand<long>;
