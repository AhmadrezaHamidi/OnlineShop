using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Users;

public interface IUserService : IDomainService
{
    Task<bool> UserExists(long id, string? phoneNumber, CancellationToken token);
}
