using Microsoft.AspNetCore.Mvc;
﻿using AhmadBase.Application;
using AhmadBase.Application.Query;
using AhmadBase.Web;
using AhmadBase.Web.Models;
using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Application.Query.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ahmad.OnlineShop.Rest.Endpoints;

public sealed class BnplCreditEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/bnpl")
            .WithTags("BNPL Credit");

        // GET /api/v1/bnpl/users/{userId}/credit
        group.MapGet("users/{userId}/credit", GetCreditLimit)
            .WithName("GetCreditLimit")
            .WithSummary("Get credit limit for a user")
            .WithDescription("Returns the current credit limit, used amount, and available credit for the specified user.");

        // PUT /api/v1/bnpl/users/{userId}/credit/increase
        group.MapPut("users/{userId}/credit/increase", IncreaseCredit)
            .WithName("IncreaseCreditLimit")
            .WithSummary("Increase the total credit limit for a user")
            .WithDescription("Sets a new (higher) total credit limit for the specified user.");

        // PATCH /api/v1/bnpl/users/{userId}/credit/block
        group.MapPatch("users/{userId}/credit/block", BlockCredit)
            .WithName("BlockCredit")
            .WithSummary("Block a credit amount for a user")
            .WithDescription("Reduces available credit by blocking the specified amount against the user's credit limit.");

        // PATCH /api/v1/bnpl/users/{userId}/credit/release
        group.MapPatch("users/{userId}/credit/release", ReleaseCredit)
            .WithName("ReleaseCredit")
            .WithSummary("Release a blocked credit amount for a user")
            .WithDescription("Restores available credit by releasing the specified previously blocked amount.");
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private static async Task<IResult> GetCreditLimit(
        long userId,
        IQueryBus queryBus,
        CancellationToken cancellation)
    {
        var result = await queryBus.DispatchAsync(new GetCreditLimitQuery(userId), cancellation);
        return Results.Ok(ApiResponse<GetCreditLimitQueryResponse>.Ok(result));
    }

    private static async Task<IResult> IncreaseCredit(
        long userId,
        [FromBody] IncreaseCreditLimitCommand command,
        ICommandBus commandBus,
        CancellationToken cancellation)
    {
        var result = await commandBus.Dispatch<long>(
            command with { UserId = userId }, cancellation);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    private static async Task<IResult> BlockCredit(
        long userId,
        [FromBody] BlockCreditCommand command,
        ICommandBus commandBus,
        CancellationToken cancellation)
    {
        var result = await commandBus.Dispatch<long>(
            command with { UserId = userId }, cancellation);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    private static async Task<IResult> ReleaseCredit(
        long userId,
        [FromBody] ReleaseCreditCommand command,
        ICommandBus commandBus,
        CancellationToken cancellation)
    {
        var result = await commandBus.Dispatch<long>(
            command with { UserId = userId }, cancellation);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }
}
