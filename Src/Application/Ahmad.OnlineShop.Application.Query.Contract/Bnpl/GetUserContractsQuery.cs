using AhmadBase.Application.Query;
using Ahmad.OnlineShop.Application.Dtos;

namespace Ahmad.OnlineShop.Application.Query.Queries;

public record GetUserContractsQuery(
    long UserId,
    int  Page,
    int  PageSize
) : IQuery<PagedResult<ContractDto>>;
