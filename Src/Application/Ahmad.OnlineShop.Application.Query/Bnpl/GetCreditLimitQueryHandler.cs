using AhmadBase.Application.Query;
using Ahmad.OnlineShop.Application.Dtos;
using Ahmad.OnlineShop.Application.Query.Queries;
using Ahmad.OnlineShop.Domain.Exceptions;
using Ahmad.OnlineShop.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Query.Handlers;

public sealed class GetCreditLimitQueryHandler : IQueryHandler<GetCreditLimitQuery, CreditLimitDto>
{
    private readonly ICreditLimitRepository _creditRepo;

    public GetCreditLimitQueryHandler(ICreditLimitRepository creditRepo)
    {
        _creditRepo = creditRepo;
    }

    public async Task<CreditLimitDto> HandleAsync(GetCreditLimitQuery query, CancellationToken token)
    {
        var credit = await _creditRepo.GetByUserIdAsync(query.UserId, token);
        if (credit is null)
        {
            var (code, msg) = BnplErrors.Get(BnplErrors.CreditLimitNotFound);
            throw new BnplDomainException(code, msg);
        }

        return new CreditLimitDto(
            credit.UserId,
            credit.TotalLimit,
            credit.UsedLimit,
            credit.AvailableLimit,
            credit.UpdatedAt);
    }
}
