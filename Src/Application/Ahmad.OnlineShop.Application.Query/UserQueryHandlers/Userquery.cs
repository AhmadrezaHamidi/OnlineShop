using Ahmad.OnlineShop.Application.Query.User;
using Ahmad.OnlineShop.Domain.Users;
using Ahmad.OnlineShop.Domain.Users.Exceptions;
using AhmadBase.Application.Query;

namespace Ahmad.OnlineShop.Application.Query.UserQueryHandlers;

public class Userquery(IUserRepository userRepository) : IQueryHandler<GetProfileQuery, GetProfileQueryResponse>
{

    public async Task<GetProfileQueryResponse> HandleAsync(GetProfileQuery query, CancellationToken token)
    {
        var user = await userRepository.Get(query.UserId, token);
        if (user is null)
            throw new UserNotFoundException();

        var result = UserMapper.Map(user);
        return result;
    }
}
