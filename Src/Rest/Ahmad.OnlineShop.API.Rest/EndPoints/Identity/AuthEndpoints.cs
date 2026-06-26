using AhmadBase.Application;
using AhmadBase.Web;
using AhmadBase.Web.Models;
using Identity.Application.Commands;
using Identity.Application.Dtos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Identity.Rest.Endpoints;

public class AuthEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth").WithTags("Auth");

        group.MapPost("/register",
            async ([FromBody] RegisterCommand cmd, ICommandBus bus, CancellationToken ct) =>
                Results.Ok(ApiResponse<long>.Ok(await bus.Dispatch<long>(cmd, ct))))
            .WithName("Auth.Register")
            .WithSummary("Register a new user")
            .Produces<ApiResponse<long>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/login",
            async ([FromBody] LoginCommand cmd, ICommandBus bus, CancellationToken ct) =>
                Results.Ok(ApiResponse<TokenResponseDto>.Ok(
                    await bus.Dispatch<TokenResponseDto>((ICommand<TokenResponseDto>)cmd, ct))))
            .WithName("Auth.Login")
            .WithSummary("Login and receive JWT tokens")
            .Produces<ApiResponse<TokenResponseDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/refresh",
            async ([FromBody] RefreshTokenCommand cmd, ICommandBus bus, CancellationToken ct) =>
                Results.Ok(ApiResponse<TokenResponseDto>.Ok(
                    await bus.Dispatch<TokenResponseDto>((ICommand<TokenResponseDto>)cmd, ct))))
            .WithName("Auth.RefreshToken")
            .WithSummary("Exchange a refresh token for new tokens")
            .Produces<ApiResponse<TokenResponseDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/logout",
            async ([FromBody] LogoutCommand cmd, ICommandBus bus, CancellationToken ct) =>
                Results.Ok(ApiResponse<bool>.Ok(await bus.Dispatch<bool>((ICommand<bool>)cmd, ct))))
            .WithName("Auth.Logout")
            .WithSummary("Revoke the current refresh token")
            .RequireAuthorization()
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK);

        group.MapPost("/change-password",
            async ([FromBody] ChangePasswordCommand cmd, ICommandBus bus, CancellationToken ct) =>
                Results.Ok(ApiResponse<bool>.Ok(await bus.Dispatch<bool>((ICommand<bool>)cmd, ct))))
            .WithName("Auth.ChangePassword")
            .WithSummary("Change the authenticated user's password")
            .RequireAuthorization()
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }
}
