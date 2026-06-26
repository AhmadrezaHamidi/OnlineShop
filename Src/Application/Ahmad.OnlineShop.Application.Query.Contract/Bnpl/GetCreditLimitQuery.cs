using AhmadBase.Application.Query;
using Ahmad.OnlineShop.Application.Dtos;

namespace Ahmad.OnlineShop.Application.Query.Queries;

public record GetCreditLimitQuery(long UserId) : IQuery<CreditLimitDto>;
