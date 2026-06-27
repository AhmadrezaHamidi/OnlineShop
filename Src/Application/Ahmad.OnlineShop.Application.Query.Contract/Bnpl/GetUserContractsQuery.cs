using AhmadBase.Application.Query;

namespace Ahmad.OnlineShop.Application.Query.Queries;

public record GetUserContractsQuery(
    long UserId,
    int  Page,
    int  PageSize
) : IQuery<QueryPagedResult<GetContractQueryResponse>>;
