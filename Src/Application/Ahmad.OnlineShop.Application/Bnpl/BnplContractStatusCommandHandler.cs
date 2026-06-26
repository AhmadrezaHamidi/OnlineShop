using AhmadBase.Application;
using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Handlers;

public sealed class BnplContractStatusCommandHandler :
    ICommandHandler<MarkContractDefaultedCommand, long>,
    ICommandHandler<CancelBnplContractCommand, long>
{
    private readonly IBnplContractRepository _contractRepo;
    private readonly ICreditLimitRepository  _creditRepo;

    public BnplContractStatusCommandHandler(
        IBnplContractRepository contractRepo,
        ICreditLimitRepository  creditRepo)
    {
        _contractRepo = contractRepo;
        _creditRepo   = creditRepo;
    }

    public async Task<long> Handle(MarkContractDefaultedCommand command, CancellationToken token)
    {
        var contract = await _contractRepo.GetByIdAsync(command.ContractId, token);
        if (contract is null)
        {
            var (code, msg) = BnplErrors.Get(BnplErrors.ContractNotFound);
            throw new BnplDomainException(code, msg);
        }

        contract.MarkDefaulted();

        await _contractRepo.UpdateAsync(contract, token);

        return contract.Id;
    }

    public async Task<long> Handle(CancelBnplContractCommand command, CancellationToken token)
    {
        var contract = await _contractRepo.GetByIdAsync(command.ContractId, token);
        if (contract is null)
        {
            var (code, msg) = BnplErrors.Get(BnplErrors.ContractNotFound);
            throw new BnplDomainException(code, msg);
        }

        // Calculate remaining (unpaid) amount to release from credit
        var remainingAmount = contract.Installments
            .Where(i => i.Status == Domain.Enums.InstallmentStatus.Pending
                     || i.Status == Domain.Enums.InstallmentStatus.Overdue)
            .Sum(i => i.Amount);

        contract.Cancel();

        // Release remaining credit back to user
        if (remainingAmount > 0)
        {
            var credit = await _creditRepo.GetByUserIdAsync(contract.UserId, token);
            if (credit is not null)
            {
                credit.Release(remainingAmount);
                await _creditRepo.UpdateAsync(credit, token);
            }
        }

        await _contractRepo.UpdateAsync(contract, token);

        return contract.Id;
    }
}
