using AhmadBase.Application.Query;

namespace Ahmad.OnlineShop.Application.Query.Queries;

public record GetInventoryQuery(long ProductId) : IQuery<GetProductInventoryResponse>;
