using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record MarkContractDefaultedCommand(
    long ContractId
) : ICommand<long>;
