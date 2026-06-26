using AhmadBase.Application.Query;
using Identity.Application.Dtos;
using Identity.Application.Query.Contracts;
using Identity.Application.Query.Queries;
using Identity.Domain.Exceptions;

namespace Identity.Application.Query.Handlers;

public class GetUserQueryHandler : IQueryHandler<GetUserQuery, UserDto>
{
    private readonly IUserReadRepository _readRepo;

    public GetUserQueryHandler(IUserReadRepository readRepo)
    {
        _readRepo = readRepo;
    }

    public async Task<UserDto> HandleAsync(GetUserQuery query, CancellationToken token)
    {
        var user = await _readRepo.GetByIdAsync(query.UserId, token);
        if (user is null)
        {
            var (code, msg) = UserErrors.Get(UserErrors.NotFound);
            throw new UserDomainException(code, msg);
        }

        return user;
    }
}
