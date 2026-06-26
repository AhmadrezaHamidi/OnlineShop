using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record ReleaseCreditCommand(
    long    UserId,
    decimal Amount
) : ICommand<long>;
