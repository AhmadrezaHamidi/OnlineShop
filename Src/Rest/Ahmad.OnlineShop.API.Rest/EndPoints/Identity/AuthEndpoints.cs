using Ahmad.OnlineShop.Rest.EndPoints.Identity;
using Identity.Application.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Rest.Endpoints;

/// <summary>
/// Endpoints احراز هویت با OTP
/// جریان: RequestOtp → دریافت SMS → VerifyOtp → JWT
/// </summary>
public class AuthEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(IdentityConstants.Routes.BaseRoute)
            .WithApiVersionSet()
            .WithTags("Auth");

        group.MapPostEndpoint(
            IdentityConstants.Routes.RequestOtp,
            RequestOtp,
            IdentityConstants.Names.RequestOtp,
            IdentityConstants.Docs.RequestOtp.Summary,
            IdentityConstants.Docs.RequestOtp.Description);

        group.MapPostEndpoint<LoginCommandResponse>(
            IdentityConstants.Routes.VerifyOtp,
            VerifyOtp,
            IdentityConstants.Names.VerifyOtp,
            IdentityConstants.Docs.VerifyOtp.Summary,
            IdentityConstants.Docs.VerifyOtp.Description);

        group.MapPostEndpoint<LoginCommandResponse>(
            IdentityConstants.Routes.RefreshToken,
            RefreshToken,
            IdentityConstants.Names.RefreshToken,
            IdentityConstants.Docs.RefreshToken.Summary,
            IdentityConstants.Docs.RefreshToken.Description);

        group.MapPostEndpoint(
            IdentityConstants.Routes.Logout,
            Logout,
            IdentityConstants.Names.Logout,
            IdentityConstants.Docs.Logout.Summary,
            IdentityConstants.Docs.Logout.Description)
            .RequireAuthorization();
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private static async Task<bool> RequestOtp(
        [FromBody] RequestOtpCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<bool>(command, ct);

    private static async Task<LoginCommandResponse> VerifyOtp(
        [FromBody] VerifyOtpCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<LoginCommandResponse>((ICommand<LoginCommandResponse>)command, ct);

    private static async Task<LoginCommandResponse> RefreshToken(
        [FromBody] RefreshTokenCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<LoginCommandResponse>((ICommand<LoginCommandResponse>)command, ct);

    private static async Task<bool> Logout(
        [FromBody] LogoutCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<bool>((ICommand<bool>)command, ct);
}
