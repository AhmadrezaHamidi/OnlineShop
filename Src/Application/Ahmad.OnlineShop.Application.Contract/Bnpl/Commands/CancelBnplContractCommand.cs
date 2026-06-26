using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record CancelBnplContractCommand(
    long ContractId
) : ICommand<long>;
