using Ahmad.OnlineShop.Application.Bnpl.Mapper;
using Ahmad.OnlineShop.Domain.Bnpl.Aggregates;
using Ahmad.OnlineShop.Domain.Bnpl.Enums;
using Ahmad.OnlineShop.Domain.Bnpl.Exceptions;
using Ahmad.OnlineShop.Persistence.EF;

namespace Ahmad.OnlineShop.Application.Handlers;

public sealed class BnplHandlers(
    IBnplContractRepository contractRepo,
    ICreditLimitRepository creditRepo,
    ApplicationDbContext context) :
    ICommandHandler<CreateBnplContractCommand, long>,
    ICommandHandler<PayInstallmentCommand, long>,
    ICommandHandler<IncreaseCreditLimitCommand, long>,
    ICommandHandler<BlockCreditCommand, long>,
    ICommandHandler<ReleaseCreditCommand, long>,
    ICommandHandler<MarkContractDefaultedCommand, long>,
    ICommandHandler<CancelBnplContractCommand, long>
{
    #region Contract Commands

    public async Task<long> Handle(CreateBnplContractCommand command, CancellationToken token)
    {
        var credit = await creditRepo.GetByUserIdAsync(command.UserId, token)
            ?? throw new CreditLimitNotFoundException();

        credit.Block(command.TotalAmount);

        var contractId = await contractRepo.GetNextIdAsync();

        var contract = BnplContract.Create(command.Map(contractId));

        await contractRepo.AddAsync(contract, token);
        await creditRepo.UpdateAsync(credit, token);
        await context.SaveChangesAsync(token);

        return contract.Id;
    }

    public async Task<long> Handle(PayInstallmentCommand command, CancellationToken token)
    {
        var contract = await contractRepo.GetByIdAsync(command.ContractId, token)
            ?? throw new BnplContractNotFoundException();

        var installment = contract.Installments.FirstOrDefault(i => i.Id == command.InstallmentId)
            ?? throw new InstallmentNotFoundException();

        var paidAmount = installment.Amount;

        contract.PayInstallment(command.InstallmentId);

        var credit = await creditRepo.GetByUserIdAsync(contract.UserId, token);
        if (credit is not null)
        {
            credit.Release(paidAmount);
            await creditRepo.UpdateAsync(credit, token);
        }

        await contractRepo.UpdateAsync(contract, token);
        await context.SaveChangesAsync(token);

        return contract.Id;
    }

    public async Task<long> Handle(MarkContractDefaultedCommand command, CancellationToken token)
    {
        var contract = await contractRepo.GetByIdAsync(command.ContractId, token)
            ?? throw new BnplContractNotFoundException();

        contract.MarkDefaulted();

        await contractRepo.UpdateAsync(contract, token);
        await context.SaveChangesAsync(token);

        return contract.Id;
    }

    public async Task<long> Handle(CancelBnplContractCommand command, CancellationToken token)
    {
        var contract = await contractRepo.GetByIdAsync(command.ContractId, token)
            ?? throw new BnplContractNotFoundException();

        var remainingAmount = contract.Installments
            .Where(i => i.Status == InstallmentStatus.Pending
                     || i.Status == InstallmentStatus.Overdue)
            .Sum(i => i.Amount);

        contract.Cancel();

        if (remainingAmount > 0)
        {
            var credit = await creditRepo.GetByUserIdAsync(contract.UserId, token);
            if (credit is not null)
            {
                credit.Release(remainingAmount);
                await creditRepo.UpdateAsync(credit, token);
            }
        }

        await contractRepo.UpdateAsync(contract, token);
        await context.SaveChangesAsync(token);

        return contract.Id;
    }

    #endregion

    #region Credit Commands

    public async Task<long> Handle(IncreaseCreditLimitCommand command, CancellationToken token)
    {
        var credit = await creditRepo.GetByUserIdAsync(command.UserId, token)
            ?? throw new CreditLimitNotFoundException();

        credit.IncreaseTotalLimit(command.NewLimit);

        await creditRepo.UpdateAsync(credit, token);
        await context.SaveChangesAsync(token);

        return credit.Id;
    }

    public async Task<long> Handle(BlockCreditCommand command, CancellationToken token)
    {
        var credit = await creditRepo.GetByUserIdAsync(command.UserId, token)
            ?? throw new CreditLimitNotFoundException();

        credit.Block(command.Amount);

        await creditRepo.UpdateAsync(credit, token);
        await context.SaveChangesAsync(token);

        return credit.Id;
    }

    public async Task<long> Handle(ReleaseCreditCommand command, CancellationToken token)
    {
        var credit = await creditRepo.GetByUserIdAsync(command.UserId, token)
            ?? throw new CreditLimitNotFoundException();

        credit.Release(command.Amount);

        await creditRepo.UpdateAsync(credit, token);
        await context.SaveChangesAsync(token);

        return credit.Id;
    }

    #endregion
}
