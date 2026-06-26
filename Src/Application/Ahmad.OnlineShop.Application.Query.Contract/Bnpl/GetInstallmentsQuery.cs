using AhmadBase.Application.Query;
using Ahmad.OnlineShop.Application.Dtos;

namespace Ahmad.OnlineShop.Application.Query.Queries;

public record GetInstallmentsQuery(long ContractId) : IQuery<List<InstallmentDto>>;
