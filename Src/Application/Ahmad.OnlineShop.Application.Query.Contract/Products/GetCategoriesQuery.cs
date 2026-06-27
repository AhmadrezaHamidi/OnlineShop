using AhmadBase.Application.Query;

namespace Ahmad.OnlineShop.Application.Query.Queries;

public record GetCategoriesQuery() : IQuery<List<GetCategoryQueryResponse>>;

public sealed record GetCategoryQueryResponse(
    long   Id,
    string Name,
    long?  ParentId);
