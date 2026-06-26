using AhmadBase.Application.Query;
using Ahmad.OnlineShop.Application.Dtos;
using Ahmad.OnlineShop.Application.Query.Queries;
using Ahmad.OnlineShop.Domain.Exceptions;
using Ahmad.OnlineShop.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Query.Handlers;

public sealed class GetInstallmentsQueryHandler : IQueryHandler<GetInstallmentsQuery, List<InstallmentDto>>
{
    private readonly IBnplContractRepository _contractRepo;

    public GetInstallmentsQueryHandler(IBnplContractRepository contractRepo)
    {
        _contractRepo = contractRepo;
    }

    public async Task<List<InstallmentDto>> HandleAsync(GetInstallmentsQuery query, CancellationToken token)
    {
        var contract = await _contractRepo.GetByIdAsync(query.ContractId, token);
        if (contract is null)
        {
            var (code, msg) = BnplErrors.Get(BnplErrors.ContractNotFound);
            throw new BnplDomainException(code, msg);
        }

        return contract.Installments
            .OrderBy(i => i.InstallmentNo)
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
    }
}
