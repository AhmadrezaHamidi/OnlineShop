using AhmadBase.Application.Query;
using Ahmad.OnlineShop.Application.Dtos;
using Ahmad.OnlineShop.Application.Query.Queries;
using Ahmad.OnlineShop.Domain.Exceptions;
using Ahmad.OnlineShop.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Query.Handlers;

public sealed class GetContractQueryHandler : IQueryHandler<GetContractQuery, ContractDto>
{
    private readonly IBnplContractRepository _contractRepo;

    public GetContractQueryHandler(IBnplContractRepository contractRepo)
    {
        _contractRepo = contractRepo;
    }

    public async Task<ContractDto> HandleAsync(GetContractQuery query, CancellationToken token)
    {
        var contract = await _contractRepo.GetByIdAsync(query.ContractId, token);
        if (contract is null)
        {
            var (code, msg) = BnplErrors.Get(BnplErrors.ContractNotFound);
            throw new BnplDomainException(code, msg);
        }

        var installments = contract.Installments
            .Select(i => new InstallmentDto(
                i.Id,
                i.ContractId,
                i.InstallmentNo,
                i.Amount,
                i.DueDate,
                i.PaidAt,
                i.Status,
                i.IsOverdue))
            .ToList();

        return new ContractDto(
            contract.Id,
            contract.OrderId,
            contract.UserId,
            contract.TotalAmount,
            contract.InstallmentCount,
            contract.Status,
            contract.CreatedAt,
            contract.PaidCount,
            contract.IsCompleted,
            installments);
    }
}
