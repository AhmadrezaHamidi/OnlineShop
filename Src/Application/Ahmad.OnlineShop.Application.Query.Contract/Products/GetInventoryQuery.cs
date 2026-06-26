using AhmadBase.Application.Query;
using Ahmad.OnlineShop.Application.Dtos;

namespace Ahmad.OnlineShop.Application.Query.Queries;

public record GetInventoryQuery(
    long ProductId
) : IQuery<InventoryDto>;
