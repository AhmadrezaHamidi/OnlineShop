using AhmadBase.Persistence.NHiLoHelper;
using Microsoft.EntityFrameworkCore;
using Ahmad.OnlineShop.Domain.User;
using Ahmad.OnlineShop.Domain.Users;

namespace Ahmad.OnlineShop.Persistence.EF.Repositories;


public class UserRepository(ApplicationDbContext context, IHiLoIdGenerator hiLoGenerator) : IUserRepository
{

    public async Task Add(User user, CancellationToken token)
        => await context.Users.AddAsync(user, token);

    public async Task<User?> Get(long id, CancellationToken token)
        => await context.Users
            .FirstOrDefaultAsync(user => user.Id == id, token);

    public async Task<User?> Get(string userName, CancellationToken token)
    => await context.Users
            .FirstOrDefaultAsync(user => user.UserName == userName, token);


    public long GetNextId()
     => hiLoGenerator.GetNextId<User?>();

    public Task Update(CancellationToken token)
        => context.SaveChangesAsync(token);
}

