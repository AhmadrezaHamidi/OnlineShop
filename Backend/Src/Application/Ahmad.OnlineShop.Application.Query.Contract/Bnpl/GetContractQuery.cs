using Ahmad.OnlineShop.Domain.Bnpl.Enums;
using AhmadBase.Application.Query;

namespace Ahmad.OnlineShop.Application.Query.Queries;

public record GetContractQuery(long ContractId) : IQuery<GetContractQueryResponse>;

public sealed record GetContractQueryResponse(
    long                         Id,
    long                         OrderId,
    long                         UserId,
    decimal                      TotalAmount,
    int                          InstallmentCount,
    ContractStatus               Status,
    DateTime                     CreatedAt,
    int                          PaidCount,
    bool                         IsCompleted,
    List<GetInstallmentResponse> Installments);

public sealed record GetInstallmentResponse(
    long              Id,
    long              ContractId,
    int               InstallmentNo,
    decimal           Amount,
    DateTime          DueDate,
    DateTime?         PaidAt,
    InstallmentStatus Status,
    bool              IsOverdue);
