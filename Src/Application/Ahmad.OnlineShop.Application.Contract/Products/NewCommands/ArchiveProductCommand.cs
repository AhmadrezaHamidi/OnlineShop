using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record ArchiveProductCommand(
    long Id
) : ICommand<long>;
