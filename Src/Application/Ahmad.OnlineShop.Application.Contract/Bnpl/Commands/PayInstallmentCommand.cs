using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record PayInstallmentCommand(
    long ContractId,
    long InstallmentId
) : ICommand<long>;
