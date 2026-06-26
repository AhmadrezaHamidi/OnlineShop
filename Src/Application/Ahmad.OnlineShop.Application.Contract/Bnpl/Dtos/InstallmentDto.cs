using Ahmad.OnlineShop.Domain.Enums;

namespace Ahmad.OnlineShop.Application.Dtos;

public record InstallmentDto(
    long              Id,
    long              ContractId,
    int               InstallmentNo,
    decimal           Amount,
    DateTime          DueDate,
    DateTime?         PaidAt,
    InstallmentStatus Status,
    bool              IsOverdue
);
