using AhmadBase.Application.Query;
using BackOffice.Application.Dtos;
using BackOffice.Application.Query.Contracts;
using BackOffice.Application.Query.Queries;

namespace BackOffice.Application.Query.Handlers;

public class GetAuditLogsQueryHandler : IQueryHandler<GetAuditLogsQuery, List<AuditLogDto>>
{
    private readonly IAdminUserReadRepository _readRepo;

    public GetAuditLogsQueryHandler(IAdminUserReadRepository readRepo)
    {
        _readRepo = readRepo;
    }

    public async Task<List<AuditLogDto>> HandleAsync(GetAuditLogsQuery query, CancellationToken token)
    {
        return await _readRepo.GetAuditLogsAsync(
            adminId:  query.AdminId,
            page:     query.Page,
            pageSize: query.PageSize,
            token:    token);
    }
}
