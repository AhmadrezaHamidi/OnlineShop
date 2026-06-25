using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Users;

public interface IUserRepository : IRepository<User.User>
{
    Task<User.User?> Get(long id, CancellationToken token);
    Task<User.User?> Get(string userName, CancellationToken token);
    Task Add(User.User user, CancellationToken token);
    Task Update(CancellationToken token);
    long GetNextId();
}




