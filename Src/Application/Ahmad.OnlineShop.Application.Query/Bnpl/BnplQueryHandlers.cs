using Ahmad.OnlineShop.Domain.Bnpl.Exceptions;

namespace Ahmad.OnlineShop.Application.Query.Handlers;

public sealed class BnplQueryHandlers(
    IBnplContractRepository contractRepo,
    ICreditLimitRepository  creditRepo) :
    IQueryHandler<GetContractQuery,      GetContractQueryResponse>,
    IQueryHandler<GetInstallmentsQuery,  List<GetInstallmentResponse>>,
    IQueryHandler<GetUserContractsQuery, QueryPagedResult<GetContractQueryResponse>>,
    IQueryHandler<GetCreditLimitQuery,   GetCreditLimitQueryResponse>
{
    public async Task<GetContractQueryResponse> HandleAsync(GetContractQuery query, CancellationToken token)
    {
        var contract = await contractRepo.GetByIdAsync(query.ContractId, token)
            ?? throw new BnplContractNotFoundException();

        return contract.ToResponse();
    }

    public async Task<List<GetInstallmentResponse>> HandleAsync(GetInstallmentsQuery query, CancellationToken token)
    {
        var contract = await contractRepo.GetByIdAsync(query.ContractId, token)
            ?? throw new BnplContractNotFoundException();

        return contract.Installments
            .OrderBy(i => i.InstallmentNo)
            .Select(i => i.ToResponse())
            .ToList();
    }

    public async Task<QueryPagedResult<GetContractQueryResponse>> HandleAsync(GetUserContractsQuery query, CancellationToken token)
    {
        var (contracts, total) = await contractRepo.GetByUserIdAsync(
            query.UserId, query.Page, query.PageSize, token);

        return new QueryPagedResult<GetContractQueryResponse>(
            contracts.Select(c => c.ToResponse()).ToList(),
            total, query.Page, query.PageSize);
    }

    public async Task<GetCreditLimitQueryResponse> HandleAsync(GetCreditLimitQuery query, CancellationToken token)
    {
        var credit = await creditRepo.GetByUserIdAsync(query.UserId, token)
            ?? throw new CreditLimitNotFoundException();

        return credit.ToResponse();
    }
}
