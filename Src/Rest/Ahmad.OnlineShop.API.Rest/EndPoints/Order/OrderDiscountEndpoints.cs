using Ahmad.OnlineShop.Application.Contract.Order.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Ahmad.OnlineShop.Rest.Endpoints;

public sealed class OrderDiscountEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v{version:apiVersion}/Order")
            .WithTags("Orders")
            .RequireAuthorization();

        group.MapPost("/{id}/discount", ApplyDiscount)
            .WithName("ApplyDiscount")
            .WithSummary("اعمال کد تخفیف به سفارش");

        group.MapPost("/{id}/package", ApplyPackage)
            .WithName("ApplyPackage")
            .WithSummary("اعمال پکیج به سفارش");
    }

    private static async Task<IResult> ApplyDiscount(
        long id,
        [FromBody] ApplyDiscountRequest request,
        ICommandBus bus,
        CancellationToken ct)
    {
        var discountAmount = await bus.Dispatch<decimal>(
            new ApplyDiscountToOrderCommand(id, request.DiscountCode), ct);
        return Results.Ok(ApiResponse<decimal>.Ok(discountAmount));
    }

    private static async Task<IResult> ApplyPackage(
        long id,
        [FromBody] ApplyPackageRequest request,
        ICommandBus bus,
        CancellationToken ct)
    {
        var discountAmount = await bus.Dispatch<decimal>(
            new ApplyPackageToOrderCommand(id, request.PackageId), ct);
        return Results.Ok(ApiResponse<decimal>.Ok(discountAmount));
    }
}

public record ApplyDiscountRequest(string DiscountCode);
public record ApplyPackageRequest(long PackageId);
