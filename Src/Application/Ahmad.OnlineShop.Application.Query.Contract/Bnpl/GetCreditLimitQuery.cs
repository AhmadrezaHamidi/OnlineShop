using AhmadBase.Application.Query;

namespace Ahmad.OnlineShop.Application.Query.Queries;

public record GetCreditLimitQuery(long UserId) : IQuery<GetCreditLimitQueryResponse>;

public sealed record GetCreditLimitQueryResponse(
    long     UserId,
    decimal  TotalLimit,
    decimal  UsedLimit,
    decimal  AvailableLimit,
    DateTime UpdatedAt);
