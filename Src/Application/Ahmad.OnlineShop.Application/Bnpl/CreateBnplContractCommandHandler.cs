using AhmadBase.Application;
using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Domain.Aggregates;
using Ahmad.OnlineShop.Domain.Exceptions;
using Ahmad.OnlineShop.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Handlers;

public sealed class CreateBnplContractCommandHandler : ICommandHandler<CreateBnplContractCommand, long>
{
    private readonly IBnplContractRepository _contractRepo;
    private readonly ICreditLimitRepository  _creditRepo;

    public CreateBnplContractCommandHandler(
        IBnplContractRepository contractRepo,
        ICreditLimitRepository  creditRepo)
    {
        _contractRepo = contractRepo;
        _creditRepo   = creditRepo;
    }

    public async Task<long> Handle(CreateBnplContractCommand command, CancellationToken token)
    {
        // Verify credit limit exists and is sufficient
        var credit = await _creditRepo.GetByUserIdAsync(command.UserId, token);
        if (credit is null)
        {
            var (code, msg) = BnplErrors.Get(BnplErrors.CreditLimitNotFound);
            throw new BnplDomainException(code, msg);
        }

        // Block the credit for the full contract amount
        credit.Block(command.TotalAmount);

        // Generate new IDs
        var contractId = await _contractRepo.GetNextIdAsync();

        // Create the contract (installments are generated inside the factory)
        var contract = BnplContract.Create(
            contractId,
            command.OrderId,
            command.UserId,
            command.TotalAmount,
            command.InstallmentCount,
            command.FirstDueDate,
            command.IntervalDays);

        await _contractRepo.AddAsync(contract, token);
        await _creditRepo.UpdateAsync(credit, token);

        return contract.Id;
    }
}
