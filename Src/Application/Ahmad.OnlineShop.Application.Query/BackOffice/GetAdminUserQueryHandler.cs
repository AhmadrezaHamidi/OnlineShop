using AhmadBase.Application.Query;
using BackOffice.Application.Dtos;
using BackOffice.Application.Query.Contracts;
using BackOffice.Application.Query.Queries;
using BackOffice.Domain.Exceptions;

namespace BackOffice.Application.Query.Handlers;

public class GetAdminUserQueryHandler : IQueryHandler<GetAdminUserQuery, AdminUserDto>
{
    private readonly IAdminUserReadRepository _readRepo;

    public GetAdminUserQueryHandler(IAdminUserReadRepository readRepo)
    {
        _readRepo = readRepo;
    }

    public async Task<AdminUserDto> HandleAsync(GetAdminUserQuery query, CancellationToken token)
    {
        var admin = await _readRepo.GetByIdAsync(query.AdminId, token);
        if (admin is null)
        {
            var (code, msg) = BackOfficeErrors.Get(BackOfficeErrors.AdminNotFound);
            throw new BackOfficeDomainException(code, msg);
        }

        return admin;
    }
}
