using Ahmad.OnlineShop.Application.Query.Queries;
using Ahmad.OnlineShop.Domain.Bnpl.Aggregates;
using Ahmad.OnlineShop.Domain.Bnpl.Entities;

namespace Ahmad.OnlineShop.Application.Query.Mappers;

internal static class BnplQueryMapper
{
    internal static GetContractQueryResponse ToResponse(this BnplContract contract)
        => new(
            contract.Id,
            contract.OrderId,
            contract.UserId,
            contract.TotalAmount,
            contract.InstallmentCount,
            contract.Status,
            contract.CreatedAt,
            contract.PaidCount,
            contract.IsCompleted,
            contract.Installments.Select(i => i.ToResponse()).ToList());

    internal static GetInstallmentResponse ToResponse(this Installment i)
        => new(i.Id, i.ContractId, i.InstallmentNo, i.Amount, i.DueDate, i.PaidAt, i.Status, i.IsOverdue);

    internal static GetCreditLimitQueryResponse ToResponse(this CreditLimit c)
        => new(c.UserId, c.TotalLimit, c.UsedLimit, c.AvailableLimit, c.UpdatedAt);
}
