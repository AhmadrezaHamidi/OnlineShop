using Ahmad.OnlineShop.API.Rest.EndPoints.Account;
using Ahmad.OnlineShop.Application.Contract.User;
using Ahmad.OnlineShop.Application.Query.User;
using AhmadBase.Application;
using AhmadBase.Application.Query;
using AhmadBase.Web;
using AhmadBase.Web.Securities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ahmad.OnlineShop.Rest.EndPoints.Account;

public sealed class AccountEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(AccountConstants.Routes.BaseRoute)
            .WithApiVersionSet()
            .WithTags("Account");

        group.MapPostEndpoint(
            AccountConstants.Routes.Register,
            RegisterAccount,
            AccountConstants.Names.Register,
            AccountConstants.Docs.Register.Summary,
            AccountConstants.Docs.Register.Description);

        group.MapPostEndpoint(
            AccountConstants.Routes.Login,
            LoginAccount,
            AccountConstants.Names.Login,
            AccountConstants.Docs.Login.Summary,
            AccountConstants.Docs.Login.Description);

        group.MapPostEndpoint(
            AccountConstants.Routes.RefreshToken,
            RefreshAccountToken,
            AccountConstants.Names.RefreshToken,
            AccountConstants.Docs.RefreshToken.Summary,
            AccountConstants.Docs.RefreshToken.Description);

        group.MapPostEndpoint(
            AccountConstants.Routes.Logout,
            LogoutAccount,
            AccountConstants.Names.Logout,
            AccountConstants.Docs.Logout.Summary,
            AccountConstants.Docs.Logout.Description);

        group.MapGetEndpoint(
            AccountConstants.Routes.GetProfile,
            GetAccountProfile,
            AccountConstants.Names.GetProfile,
            AccountConstants.Docs.GetProfile.Summary,
            AccountConstants.Docs.GetProfile.Description);

        group.MapPatchEndpoint(
            AccountConstants.Routes.UpdateProfile,
            UpdateAccountProfile,
            AccountConstants.Names.UpdateProfile,
            AccountConstants.Docs.UpdateProfile.Summary,
            AccountConstants.Docs.UpdateProfile.Description);

        group.MapPatchEndpoint(
            AccountConstants.Routes.ChangePassword,
            ChangeAccountPassword,
            AccountConstants.Names.ChangePassword,
            AccountConstants.Docs.ChangePassword.Summary,
            AccountConstants.Docs.ChangePassword.Description);

        //group.MapGetEndpoint(
        //    AccountConstants.Routes.GetAccounts,
        //    GetAccounts,
        //    AccountConstants.Names.GetAccounts,
        //    AccountConstants.Docs.GetAccounts.Summary,
        //    AccountConstants.Docs.GetAccounts.Description);

        //group.MapGetEndpoint(
        //    AccountConstants.Routes.GetAccountConditions,
        //    GetAccountConditions,
        //    AccountConstants.Names.GetAccountConditions,
        //    AccountConstants.Docs.GetAccountConditions.Summary,
        //    AccountConstants.Docs.GetAccountConditions.Description);
    }

    private static async Task<long> RegisterAccount(
        RegisterCommand command,
        ICommandBus sender,
        CancellationToken cancellation)
        => await sender.Dispatch<long>(command, cancellation);

    private static async Task<TokenResponse> LoginAccount(
        LoginCommand command,
        ICommandBus sender,
        CancellationToken cancellation)
        => await sender.Dispatch<TokenResponse>(command, cancellation);

    private static async Task<TokenResponse> RefreshAccountToken(
        RefreshTokenCommand command,
        ICommandBus sender,
        CancellationToken cancellation)
        => await sender.Dispatch<TokenResponse>(command, cancellation);

    private static async Task<long> LogoutAccount(
        LogoutCommand command,
        ICommandBus sender,
        CancellationToken cancellation)
        => await sender.Dispatch<long>(command, cancellation);

    private static async Task<GetProfileQueryResponse> GetAccountProfile(
        IQueryBus queryBus,
        HttpContext httpContext,
        CancellationToken cancellation)
    {
        var userId = httpContext.GetUserId();
        return await queryBus.DispatchAsync(new GetProfileQuery(userId), cancellation);
    }

    private static async Task<long> UpdateAccountProfile(
        UpdateProfileCommand command,
        ICommandBus sender,
        CancellationToken cancellation)
        => await sender.Dispatch<long>(command, cancellation);

    private static async Task<long> ChangeAccountPassword(
        ChangePasswordCommand command,
        ICommandBus sender,
        CancellationToken cancellation)
        => await sender.Dispatch<long>(command, cancellation);

    //private static async Task<IResult> GetAccounts(
    //    IQueryBus queryBus,
    //    CancellationToken cancellation)
    //    => (await queryBus.Dispatch<IResult>(new GetAccountsQuery(), cancellation)).ToHttpResult();

    //private static async Task<IResult> GetAccountConditions(
    //    string accountNo,
    //    IQueryBus queryBus,
    //    CancellationToken cancellation)
    //    => (await queryBus.Dispatch<IResult>(new GetAccountConditionsQuery(accountNo), cancellation)).ToHttpResult();
}