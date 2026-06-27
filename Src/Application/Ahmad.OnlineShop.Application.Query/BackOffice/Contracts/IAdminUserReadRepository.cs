using Ahmad.OnlineShop.Domain.BackOffice.Enums;
using BackOffice.Application.Query.Queries;

namespace BackOffice.Application.Query.Contracts;

public interface IAdminUserReadRepository
{
    Task<GetAdminUserQueryResponse?> GetByIdAsync(long adminId, CancellationToken token = default);

    Task<(List<GetAdminUserQueryResponse> Items, int Total)> GetListAsync(
        int          page,
        int          pageSize,
        AdminStatus? status,
        AdminRole?   role,
        CancellationToken token = default);

    Task<List<GetReportQueryResponse>> GetReportsAsync(
        long adminId,
        int  page,
        int  pageSize,
        CancellationToken token = default);

    Task<List<GetAuditLogQueryResponse>> GetAuditLogsAsync(
        long adminId,
        int  page,
        int  pageSize,
        CancellationToken token = default);
}
