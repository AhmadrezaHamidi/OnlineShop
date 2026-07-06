using AhmadBase.Application.Query;

namespace Ahmad.OnlineShop.Application.Query.Queries;

public record GetProductImagesQuery(long ProductId) : IQuery<List<GetProductImageResponse>>;
