using AhmadBase.Application.Query;
using Ahmad.OnlineShop.Application.Dtos;
using Ahmad.OnlineShop.Application.Query.Queries;
using Ahmad.OnlineShop.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Query.Handlers;

public sealed class GetUserContractsQueryHandler : IQueryHandler<GetUserContractsQuery, PagedResult<ContractDto>>
{
    private readonly IBnplContractRepository _contractRepo;

    public GetUserContractsQueryHandler(IBnplContractRepository contractRepo)
    {
        _contractRepo = contractRepo;
    }

    public async Task<PagedResult<ContractDto>> HandleAsync(GetUserContractsQuery query, CancellationToken token)
    {
        var (contracts, total) = await _contractRepo.GetByUserIdAsync(query.UserId, query.Page, query.PageSize, token);

        var items = contracts.Select(contract =>
        {
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
        }).ToList();

        return new PagedResult<ContractDto>(items, total, query.Page, query.PageSize);
    }
}
