using Ahmad.OnlineShop.Application.Commands.Discount;
using Ahmad.OnlineShop.Application.Query.Queries.Discount;
using Ahmad.OnlineShop.Rest.EndPoints.Discount;
using Microsoft.AspNetCore.Mvc;

namespace Ahmad.OnlineShop.Rest.EndPoints.Discounts;

/// <summary>
/// مدیریت کدهای تخفیف
/// جریان: ایجاد → فعال‌سازی → اعمال روی سفارش
/// </summary>
public sealed class DiscountEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(DiscountConstants.Routes.Base)
            .WithApiVersionSet()
            .WithTags(DiscountConstants.Tags.Discount)
            .RequireAuthorization();

        group.MapPost(string.Empty, Create)
            .WithName("CreateDiscount").WithSummary("ایجاد کد تخفیف");

        group.MapGet(DiscountConstants.Routes.ById, GetById)
            .WithName("GetDiscountById").WithSummary("دریافت تخفیف");

        group.MapGet(DiscountConstants.Routes.ByCode, GetByCode)
            .WithName("GetDiscountByCode").WithSummary("دریافت تخفیف با کد");

        group.MapGet(string.Empty, GetList)
            .WithName("GetDiscounts").WithSummary("لیست تخفیف‌ها");

        group.MapPut(DiscountConstants.Routes.Activate, Activate)
            .WithName("ActivateDiscount").WithSummary("فعال‌سازی تخفیف");

        group.MapPut(DiscountConstants.Routes.Deactivate, Deactivate)
            .WithName("DeactivateDiscount").WithSummary("غیرفعال‌سازی تخفیف");

        group.MapPost(DiscountConstants.Routes.Apply, Apply)
            .WithName("ApplyDiscount").WithSummary("اعمال کد تخفیف");
    }

    private static async Task<IResult> Create(
        [FromBody] CreateDiscountCommand cmd,
        ICommandBus bus, CancellationToken ct)
    {
        var result = await bus.Dispatch<long>(cmd, ct);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    private static async Task<IResult> GetById(
        long id, IQueryBus queryBus, CancellationToken ct)
    {
        var result = await queryBus.DispatchAsync<GetDiscountQueryResponse?>(new GetDiscountQuery(id), ct);
        return Results.Ok(ApiResponse<GetDiscountQueryResponse?>.Ok(result));
    }

    private static async Task<IResult> GetByCode(
        string code, IQueryBus queryBus, CancellationToken ct)
    {
        var result = await queryBus.DispatchAsync<GetDiscountQueryResponse?>(new GetDiscountByCodeQuery(code), ct);
        return Results.Ok(ApiResponse<GetDiscountQueryResponse?>.Ok(result));
    }

    private static async Task<IResult> GetList(
        IQueryBus queryBus, CancellationToken ct,
        int page = 1, int pageSize = 20, bool? isActive = null)
    {
        var result = await queryBus.DispatchAsync<PagedResult<GetDiscountQueryResponse>>(
            new GetDiscountsQuery(page, pageSize, isActive), ct);
        return Results.Ok(ApiResponse<PagedResult<GetDiscountQueryResponse>>.Ok(result));
    }

    private static async Task<IResult> Activate(
        long id, ICommandBus bus, CancellationToken ct)
    {
        var result = await bus.Dispatch<bool>(new ActivateDiscountCommand(id), ct);
        return Results.Ok(ApiResponse<bool>.Ok(result));
    }

    private static async Task<IResult> Deactivate(
        long id, ICommandBus bus, CancellationToken ct)
    {
        var result = await bus.Dispatch<bool>(new DeactivateDiscountCommand(id), ct);
        return Results.Ok(ApiResponse<bool>.Ok(result));
    }

    private static async Task<IResult> Apply(
        [FromBody] ApplyDiscountCommand cmd,
        ICommandBus bus, CancellationToken ct)
    {
        var result = await bus.Dispatch<decimal>(cmd, ct);
        return Results.Ok(ApiResponse<decimal>.Ok(result));
    }
}
