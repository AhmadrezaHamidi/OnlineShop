using BackOffice.Application.Query.Contracts;
using BackOffice.Application.Query.Queries;
using Ahmad.OnlineShop.Domain.BackOffice.Exceptions;

namespace BackOffice.Application.Query.Handlers;

public class BackOfficeQueryHandlers(IAdminUserReadRepository readRepo) :
    IQueryHandler<GetAdminUserQuery,  GetAdminUserQueryResponse>,
    IQueryHandler<GetAdminUsersQuery, BackOfficePagedResult<GetAdminUserQueryResponse>>,
    IQueryHandler<GetAuditLogsQuery,  List<GetAuditLogQueryResponse>>,
    IQueryHandler<GetReportsQuery,    List<GetReportQueryResponse>>
{
    public async Task<GetAdminUserQueryResponse> HandleAsync(GetAdminUserQuery query, CancellationToken token)
    {
        var admin = await readRepo.GetByIdAsync(query.AdminId, token)
            ?? throw new AdminNotFoundException();

        return admin;
    }

    public async Task<BackOfficePagedResult<GetAdminUserQueryResponse>> HandleAsync(GetAdminUsersQuery query, CancellationToken token)
    {
        var (items, total) = await readRepo.GetListAsync(
            page:     query.Page,
            pageSize: query.PageSize,
            status:   query.Status,
            role:     query.Role,
            token:    token);

        return new BackOfficePagedResult<GetAdminUserQueryResponse>(
            Items:      items,
            TotalCount: total,
            Page:       query.Page,
            PageSize:   query.PageSize);
    }

    public async Task<List<GetAuditLogQueryResponse>> HandleAsync(GetAuditLogsQuery query, CancellationToken token)
        => await readRepo.GetAuditLogsAsync(query.AdminId, query.Page, query.PageSize, token);

    public async Task<List<GetReportQueryResponse>> HandleAsync(GetReportsQuery query, CancellationToken token)
        => await readRepo.GetReportsAsync(query.AdminId, query.Page, query.PageSize, token);
}
