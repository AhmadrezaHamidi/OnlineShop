using Ahmad.OnlineShop.Application.Query.Mappers.Discount;
using Ahmad.OnlineShop.Application.Query.Queries.Discount;
using Ahmad.OnlineShop.Domain.Discount.Exceptions;
using Ahmad.OnlineShop.Domain.Discount.Repositories;

namespace Ahmad.OnlineShop.Application.Query.Handlers.Discount;

public sealed class DiscountQueryHandlers(
    IDiscountRepository       discountRepo,
    IProductPackageRepository packageRepo) :
    IQueryHandler<GetDiscountQuery,     GetDiscountQueryResponse?>,
    IQueryHandler<GetDiscountByCodeQuery, GetDiscountQueryResponse?>,
    IQueryHandler<GetDiscountsQuery,    PagedResult<GetDiscountQueryResponse>>,
    IQueryHandler<GetPackageQuery,      GetPackageQueryResponse?>,
    IQueryHandler<GetPackagesQuery,     PagedResult<GetPackageQueryResponse>>
{
    public async Task<GetDiscountQueryResponse?> HandleAsync(GetDiscountQuery query, CancellationToken token)
    {
        var d = await discountRepo.GetByIdAsync(query.DiscountId, token);
        return d?.ToResponse();
    }

    public async Task<GetDiscountQueryResponse?> HandleAsync(GetDiscountByCodeQuery query, CancellationToken token)
    {
        var d = await discountRepo.GetByCodeAsync(query.Code, token);
        return d?.ToResponse();
    }

    public async Task<PagedResult<GetDiscountQueryResponse>> HandleAsync(GetDiscountsQuery query, CancellationToken token)
    {
        var (items, total) = await discountRepo.GetListAsync(
            query.Page, query.PageSize, query.IsActive, token);

        return new PagedResult<GetDiscountQueryResponse>(
            items.Select(d => d.ToResponse()).ToList(),
            total, query.Page, query.PageSize);
    }

    public async Task<GetPackageQueryResponse?> HandleAsync(GetPackageQuery query, CancellationToken token)
    {
        var p = await packageRepo.GetByIdAsync(query.PackageId, token);
        return p?.ToResponse();
    }

    public async Task<PagedResult<GetPackageQueryResponse>> HandleAsync(GetPackagesQuery query, CancellationToken token)
    {
        var (items, total) = await packageRepo.GetListAsync(
            query.Page, query.PageSize, query.IsActive, token);

        return new PagedResult<GetPackageQueryResponse>(
            items.Select(p => p.ToResponse()).ToList(),
            total, query.Page, query.PageSize);
    }
}
