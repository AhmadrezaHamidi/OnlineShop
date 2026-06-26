using BackOffice.Application.Dtos;
using BackOffice.Domain.Enums;

namespace BackOffice.Application.Query.Contracts;

/// <summary>
/// Read-side (query) repository for admin users.
/// Implemented in the Infrastructure/Read layer (e.g. Dapper).
/// </summary>
public interface IAdminUserReadRepository
{
    Task<AdminUserDto?> GetByIdAsync(long adminId, CancellationToken token = default);

    Task<(List<AdminUserDto> Items, int Total)> GetListAsync(
        int          page,
        int          pageSize,
        AdminStatus? status,
        AdminRole?   role,
        CancellationToken token = default);

    Task<List<ReportDto>> GetReportsAsync(
        long adminId,
        int  page,
        int  pageSize,
        CancellationToken token = default);

    Task<List<AuditLogDto>> GetAuditLogsAsync(
        long adminId,
        int  page,
        int  pageSize,
        CancellationToken token = default);
}
