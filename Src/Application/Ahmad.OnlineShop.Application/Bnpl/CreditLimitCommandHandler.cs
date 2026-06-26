using AhmadBase.Application;
using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Domain.Exceptions;
using Ahmad.OnlineShop.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Handlers;

/// <summary>
/// Handles IncreaseCreditLimitCommand, BlockCreditCommand, and ReleaseCreditCommand.
/// All three mutate the CreditLimit aggregate for a given user.
/// </summary>
public sealed class CreditLimitCommandHandler :
    ICommandHandler<IncreaseCreditLimitCommand, long>,
    ICommandHandler<BlockCreditCommand, long>,
    ICommandHandler<ReleaseCreditCommand, long>
{
    private readonly ICreditLimitRepository _creditRepo;

    public CreditLimitCommandHandler(ICreditLimitRepository creditRepo)
    {
        _creditRepo = creditRepo;
    }

    public async Task<long> Handle(IncreaseCreditLimitCommand command, CancellationToken token)
    {
        var credit = await _creditRepo.GetByUserIdAsync(command.UserId, token);
        if (credit is null)
        {
            var (code, msg) = BnplErrors.Get(BnplErrors.CreditLimitNotFound);
            throw new BnplDomainException(code, msg);
        }

        credit.IncreaseTotalLimit(command.NewLimit);
        await _creditRepo.UpdateAsync(credit, token);

        return credit.Id;
    }

    public async Task<long> Handle(BlockCreditCommand command, CancellationToken token)
    {
        var credit = await _creditRepo.GetByUserIdAsync(command.UserId, token);
        if (credit is null)
        {
            var (code, msg) = BnplErrors.Get(BnplErrors.CreditLimitNotFound);
            throw new BnplDomainException(code, msg);
        }

        credit.Block(command.Amount);
        await _creditRepo.UpdateAsync(credit, token);

        return credit.Id;
    }

    public async Task<long> Handle(ReleaseCreditCommand command, CancellationToken token)
    {
        var credit = await _creditRepo.GetByUserIdAsync(command.UserId, token);
        if (credit is null)
        {
            var (code, msg) = BnplErrors.Get(BnplErrors.CreditLimitNotFound);
            throw new BnplDomainException(code, msg);
        }

        credit.Release(command.Amount);
        await _creditRepo.UpdateAsync(credit, token);

        return credit.Id;
    }
}
