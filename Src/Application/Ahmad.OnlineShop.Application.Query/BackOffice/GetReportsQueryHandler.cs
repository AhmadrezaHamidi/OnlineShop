using AhmadBase.Application.Query;
using BackOffice.Application.Dtos;
using BackOffice.Application.Query.Contracts;
using BackOffice.Application.Query.Queries;

namespace BackOffice.Application.Query.Handlers;

public class GetReportsQueryHandler : IQueryHandler<GetReportsQuery, List<ReportDto>>
{
    private readonly IAdminUserReadRepository _readRepo;

    public GetReportsQueryHandler(IAdminUserReadRepository readRepo)
    {
        _readRepo = readRepo;
    }

    public async Task<List<ReportDto>> HandleAsync(GetReportsQuery query, CancellationToken token)
    {
        return await _readRepo.GetReportsAsync(
            adminId:  query.AdminId,
            page:     query.Page,
            pageSize: query.PageSize,
            token:    token);
    }
}
