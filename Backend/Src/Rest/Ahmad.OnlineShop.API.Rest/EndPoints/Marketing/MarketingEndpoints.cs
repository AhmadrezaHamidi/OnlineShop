using Ahmad.OnlineShop.Application.Commands.Marketing;
using Microsoft.AspNetCore.Mvc;

namespace Ahmad.OnlineShop.Rest.EndPoints.Marketing;

/// <summary>
/// Endpoint های بازاریابی — ارسال پیامک انبوه به مشتریان
/// </summary>
public sealed class MarketingEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v{version:apiVersion}/Marketing")
            .WithApiVersionSet()
            .WithTags("Marketing")
            .RequireAuthorization();

        group.MapPost("sms/broadcast", SendBulkSms)
            .WithName("SendBulkSms")
            .WithSummary("ارسال پیامک انبوه به همه مشتریان");
    }

    private static async Task<IResult> SendBulkSms(
        [FromBody] SendBulkSmsCommand cmd,
        ICommandBus bus, CancellationToken ct)
    {
        var result = await bus.Dispatch<int>(cmd, ct);
        return Results.Ok(ApiResponse<int>.Ok(result));
    }
}
