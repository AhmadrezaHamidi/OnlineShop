
using Base.Application.Common;
using Base.Application.Dtos.Auth;
namespace Base.Application.Contracts
{
    public interface IUserService
    {
        Task<OperationResult<TokenDto>> GetToken(string username, string password);
        Task<OperationResult<TokenDto>> GetRefreshToken(string refreshToken);

        Task<OperationResult<UserDto>> GetById(int id);
        Task<OperationResult<List<UserListDto>>> GetAll();
        Task<OperationResult<CreateResultDto>> Create(UserCreateDto user, string role);
        Task<OperationResult<bool>> Delete(int id);
        Task<OperationResult<bool>> Update(UserUpdateDto model);
    }
}
