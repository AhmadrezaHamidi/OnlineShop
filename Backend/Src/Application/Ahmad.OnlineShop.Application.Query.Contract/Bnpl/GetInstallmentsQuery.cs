using AhmadBase.Application.Query;

namespace Ahmad.OnlineShop.Application.Query.Queries;

public record GetInstallmentsQuery(long ContractId) : IQuery<List<GetInstallmentResponse>>;
