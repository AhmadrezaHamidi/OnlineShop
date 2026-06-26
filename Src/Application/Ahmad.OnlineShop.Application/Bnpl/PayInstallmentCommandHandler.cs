using AhmadBase.Application;
using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Domain.Exceptions;
using Ahmad.OnlineShop.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Handlers;

public sealed class PayInstallmentCommandHandler : ICommandHandler<PayInstallmentCommand, long>
{
    private readonly IBnplContractRepository _contractRepo;
    private readonly ICreditLimitRepository  _creditRepo;

    public PayInstallmentCommandHandler(
        IBnplContractRepository contractRepo,
        ICreditLimitRepository  creditRepo)
    {
        _contractRepo = contractRepo;
        _creditRepo   = creditRepo;
    }

    public async Task<long> Handle(PayInstallmentCommand command, CancellationToken token)
    {
        var contract = await _contractRepo.GetByIdAsync(command.ContractId, token);
        if (contract is null)
        {
            var (code, msg) = BnplErrors.Get(BnplErrors.ContractNotFound);
            throw new BnplDomainException(code, msg);
        }

        // Find the installment amount before paying (needed for credit release)
        var installment = contract.Installments.FirstOrDefault(i => i.Id == command.InstallmentId);
        if (installment is null)
        {
            var (code, msg) = BnplErrors.Get(BnplErrors.InstallmentNotFound);
            throw new BnplDomainException(code, msg);
        }

        var paidAmount = installment.Amount;

        // Domain: pay installment (may complete the contract)
        contract.PayInstallment(command.InstallmentId);

        // Release the corresponding credit amount
        var credit = await _creditRepo.GetByUserIdAsync(contract.UserId, token);
        if (credit is not null)
        {
            credit.Release(paidAmount);
            await _creditRepo.UpdateAsync(credit, token);
        }

        await _contractRepo.UpdateAsync(contract, token);

        return contract.Id;
    }
}
