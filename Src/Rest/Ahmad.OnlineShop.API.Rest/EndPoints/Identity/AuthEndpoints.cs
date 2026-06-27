using Ahmad.OnlineShop.Rest.EndPoints.Identity;
using Identity.Application.Commands;

namespace Identity.Rest.Endpoints;

public class AuthEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(IdentityConstants.Routes.BaseRoute)
            .WithApiVersionSet()
            .WithTags("Auth");

        group.MapPostEndpoint(
            IdentityConstants.Routes.Register,
            Register,
            IdentityConstants.Names.Register,
            IdentityConstants.Docs.Register.Summary,
            IdentityConstants.Docs.Register.Description);

        group.MapPostEndpoint<LoginCommandResponse>(
            IdentityConstants.Routes.Login,
            Login,
            IdentityConstants.Names.Login,
            IdentityConstants.Docs.Login.Summary,
            IdentityConstants.Docs.Login.Description);

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

        group.MapPostEndpoint(
            IdentityConstants.Routes.ChangePassword,
            ChangePassword,
            IdentityConstants.Names.ChangePassword,
            IdentityConstants.Docs.ChangePassword.Summary,
            IdentityConstants.Docs.ChangePassword.Description)
            .RequireAuthorization();
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private static async Task<long> Register(
        RegisterCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<long>(command, ct);

    private static async Task<LoginCommandResponse> Login(
        LoginCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<LoginCommandResponse>((ICommand<LoginCommandResponse>)command, ct);

    private static async Task<LoginCommandResponse> RefreshToken(
        RefreshTokenCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<LoginCommandResponse>((ICommand<LoginCommandResponse>)command, ct);

    private static async Task<bool> Logout(
        LogoutCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<bool>((ICommand<bool>)command, ct);

    private static async Task<bool> ChangePassword(
        ChangePasswordCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<bool>((ICommand<bool>)command, ct);
}
