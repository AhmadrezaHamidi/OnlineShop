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

public sealed class BnplContractEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/bnpl")
            .WithTags("BNPL Contracts");

        // POST /api/v1/bnpl/contracts
        group.MapPost("contracts", CreateContract)
            .WithName("CreateBnplContract")
            .WithSummary("Create a new BNPL contract")
            .WithDescription("Creates a BNPL contract for an order with auto-generated installment schedule.");

        // POST /api/v1/bnpl/contracts/{id}/installments/{installmentId}/pay
        group.MapPost("contracts/{id}/installments/{installmentId}/pay", PayInstallment)
            .WithName("PayInstallment")
            .WithSummary("Pay a specific installment")
            .WithDescription("Marks the specified installment as paid and releases the corresponding credit.");

        // PATCH /api/v1/bnpl/contracts/{id}/default
        group.MapPatch("contracts/{id}/default", DefaultContract)
            .WithName("MarkContractDefaulted")
            .WithSummary("Mark a contract as defaulted")
            .WithDescription("Transitions the contract status to Defaulted.");

        // PATCH /api/v1/bnpl/contracts/{id}/cancel
        group.MapPatch("contracts/{id}/cancel", CancelContract)
            .WithName("CancelBnplContract")
            .WithSummary("Cancel a BNPL contract")
            .WithDescription("Cancels the contract and releases remaining unpaid credit back to the user.");

        // GET /api/v1/bnpl/contracts/{id}
        group.MapGet("contracts/{id}", GetContract)
            .WithName("GetBnplContract")
            .WithSummary("Get a BNPL contract by ID")
            .WithDescription("Returns the full contract including all installments.");

        // GET /api/v1/bnpl/users/{userId}/contracts
        group.MapGet("users/{userId}/contracts", GetUserContracts)
            .WithName("GetUserContracts")
            .WithSummary("Get paginated contracts for a user")
            .WithDescription("Returns a paged list of all BNPL contracts belonging to the specified user.");

        // GET /api/v1/bnpl/contracts/{id}/installments
        group.MapGet("contracts/{id}/installments", GetInstallments)
            .WithName("GetContractInstallments")
            .WithSummary("Get all installments for a contract")
            .WithDescription("Returns all installments for the specified contract ordered by installment number.");
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private static async Task<IResult> CreateContract(
        [FromBody] CreateBnplContractCommand command,
        ICommandBus commandBus,
        CancellationToken cancellation)
    {
        var result = await commandBus.Dispatch<long>(command, cancellation);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    private static async Task<IResult> PayInstallment(
        long id,
        long installmentId,
        ICommandBus commandBus,
        CancellationToken cancellation)
    {
        var result = await commandBus.Dispatch<long>(
            new PayInstallmentCommand(id, installmentId), cancellation);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    private static async Task<IResult> DefaultContract(
        long id,
        ICommandBus commandBus,
        CancellationToken cancellation)
    {
        var result = await commandBus.Dispatch<long>(
            new MarkContractDefaultedCommand(id), cancellation);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    private static async Task<IResult> CancelContract(
        long id,
        ICommandBus commandBus,
        CancellationToken cancellation)
    {
        var result = await commandBus.Dispatch<long>(
            new CancelBnplContractCommand(id), cancellation);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    private static async Task<IResult> GetContract(
        long id,
        IQueryBus queryBus,
        CancellationToken cancellation)
    {
        var result = await queryBus.DispatchAsync(new GetContractQuery(id), cancellation);
        return Results.Ok(ApiResponse<GetContractQueryResponse>.Ok(result));
    }

    private static async Task<IResult> GetUserContracts(
        long userId,
        int page,
        int pageSize,
        IQueryBus queryBus,
        CancellationToken cancellation)
    {
        var result = await queryBus.DispatchAsync(
            new GetUserContractsQuery(userId, page, pageSize), cancellation);
        return Results.Ok(ApiResponse<PagedResult<GetContractQueryResponse>>.Ok(result));
    }

    private static async Task<IResult> GetInstallments(
        long id,
        IQueryBus queryBus,
        CancellationToken cancellation)
    {
        var result = await queryBus.DispatchAsync(new GetInstallmentsQuery(id), cancellation);
        return Results.Ok(ApiResponse<List<GetInstallmentResponse>>.Ok(result));
    }
}

