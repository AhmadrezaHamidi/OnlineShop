using Ahmad.OnlineShop.Domain.BackOffice.Aggregates;
using Ahmad.OnlineShop.Domain.BackOffice.Enums;
using Ahmad.OnlineShop.Domain.User;
using AhmadBase.Persistence.NHiLoHelper;
using BackOffice.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ahmad.OnlineShop.Persistence.EF.Repositories;

public class AdminUserRepository(ApplicationDbContext context, IHiLoIdGenerator hiLoGenerator) : IAdminUserRepository
{
    public async Task<AdminUser?> GetByIdAsync(long id, CancellationToken token = default)
        => await context.AdminUsers
            .Include(x => x.AuditLogs)
            .Include(x => x.Reports)
            .FirstOrDefaultAsync(x => x.Id == id, token);

    public async Task<AdminUser?> GetByEmailAsync(string email, CancellationToken token = default)
        => await context.AdminUsers
            .FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower(), token);

    public async Task<(List<AdminUser> Items, int Total)> GetListAsync(
        int page,
        int pageSize,
        AdminStatus? status = null,
        AdminRole? role = null,
        CancellationToken token = default)
    {
        var query = context.AdminUsers.AsQueryable();

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (role.HasValue)
            query = query.Where(x => x.Role == role.Value);

        var total = await query.CountAsync(token);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(token);

        return (items, total);
    }

    public async Task AddAsync(AdminUser admin, CancellationToken token = default) => await context.AdminUsers.AddAsync(admin, token);

    public Task UpdateAsync(AdminUser admin, CancellationToken token = default)
    {
        context.AdminUsers.Update(admin);
        return Task.CompletedTask;
    }


    public long GetNextId()
      => hiLoGenerator.GetNextId<User?>();
}
