using Ahmad.OnlineShop.Application.Commands.Discount;
using Ahmad.OnlineShop.Application.Query.Queries.Discount;
using Ahmad.OnlineShop.Rest.EndPoints.Discount;
using Microsoft.AspNetCore.Mvc;

namespace Ahmad.OnlineShop.Rest.EndPoints.Discounts;

/// <summary>
/// مدیریت پکیج‌های محصول با تخفیف
/// جریان: ایجاد → افزودن محصولات → فعال‌سازی
/// </summary>
public sealed class PackageEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(DiscountConstants.Routes.PackagesBase)
            .WithApiVersionSet()
            .WithTags(DiscountConstants.Tags.Package)
            .RequireAuthorization();

        group.MapPost(string.Empty, Create)
            .WithName("CreatePackage").WithSummary("ایجاد پکیج");

        group.MapGet(DiscountConstants.Routes.PackageById, GetById)
            .WithName("GetPackageById").WithSummary("دریافت پکیج");

        group.MapGet(string.Empty, GetList)
            .WithName("GetPackages").WithSummary("لیست پکیج‌ها");

        group.MapPost(DiscountConstants.Routes.PackageItems, AddItem)
            .WithName("AddPackageItem").WithSummary("افزودن محصول به پکیج");

        group.MapDelete(DiscountConstants.Routes.PackageItems + "/{productId:long}", RemoveItem)
            .WithName("RemovePackageItem").WithSummary("حذف محصول از پکیج");

        group.MapPut(DiscountConstants.Routes.PackageActivate, Activate)
            .WithName("ActivatePackage").WithSummary("فعال‌سازی پکیج");

        group.MapPut(DiscountConstants.Routes.PackageDeactivate, Deactivate)
            .WithName("DeactivatePackage").WithSummary("غیرفعال‌سازی پکیج");
    }

    private static async Task<IResult> Create(
        [FromBody] CreatePackageCommand cmd,
        ICommandBus bus, CancellationToken ct)
    {
        var result = await bus.Dispatch<long>(cmd, ct);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    private static async Task<IResult> GetById(
        long id, IQueryBus queryBus, CancellationToken ct)
    {
        var result = await queryBus.DispatchAsync<GetPackageQueryResponse?>(new GetPackageQuery(id), ct);
        return Results.Ok(ApiResponse<GetPackageQueryResponse?>.Ok(result));
    }

    private static async Task<IResult> GetList(
        IQueryBus queryBus, CancellationToken ct,
        int page = 1, int pageSize = 20, bool? isActive = null)
    {
        var result = await queryBus.DispatchAsync<PagedResult<GetPackageQueryResponse>>(
            new GetPackagesQuery(page, pageSize, isActive), ct);
        return Results.Ok(ApiResponse<PagedResult<GetPackageQueryResponse>>.Ok(result));
    }

    private static async Task<IResult> AddItem(
        long id,
        [FromBody] AddPackageItemCommand cmd,
        ICommandBus bus, CancellationToken ct)
    {
        var result = await bus.Dispatch<bool>(cmd with { PackageId = id }, ct);
        return Results.Ok(ApiResponse<bool>.Ok(result));
    }

    private static async Task<IResult> RemoveItem(
        long id, long productId,
        ICommandBus bus, CancellationToken ct)
    {
        var result = await bus.Dispatch<bool>(new RemovePackageItemCommand(id, productId), ct);
        return Results.Ok(ApiResponse<bool>.Ok(result));
    }

    private static async Task<IResult> Activate(
        long id, ICommandBus bus, CancellationToken ct)
    {
        var result = await bus.Dispatch<bool>(new ActivatePackageCommand(id), ct);
        return Results.Ok(ApiResponse<bool>.Ok(result));
    }

    private static async Task<IResult> Deactivate(
        long id, ICommandBus bus, CancellationToken ct)
    {
        var result = await bus.Dispatch<bool>(new DeactivatePackageCommand(id), ct);
        return Results.Ok(ApiResponse<bool>.Ok(result));
    }
}
