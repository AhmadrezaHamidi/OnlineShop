
using Ahmad.OnlineShop.Domain.BackOffice.Aggregates;
using Ahmad.OnlineShop.Domain.BackOffice.Enums;

namespace BackOffice.Domain.Repositories;

public interface IAdminUserRepository
{
    Task<AdminUser?> GetByIdAsync(long id, CancellationToken token = default);
    Task<AdminUser?> GetByEmailAsync(string email, CancellationToken token = default);
    Task<(List<AdminUser> Items, int Total)> GetListAsync(int page, int pageSize, AdminStatus? status, AdminRole? role, CancellationToken token = default);
    Task AddAsync(AdminUser admin, CancellationToken token = default);
    Task UpdateAsync(AdminUser admin, CancellationToken token = default);
    public long GetNextId();
}
