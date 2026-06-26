using Ahmad.OnlineShop.Domain.Enums;

namespace Ahmad.OnlineShop.Application.Dtos;

public record ContractDto(
    long                 Id,
    long                 OrderId,
    long                 UserId,
    decimal              TotalAmount,
    int                  InstallmentCount,
    ContractStatus       Status,
    DateTime             CreatedAt,
    int                  PaidCount,
    bool                 IsCompleted,
    List<InstallmentDto> Installments
);
