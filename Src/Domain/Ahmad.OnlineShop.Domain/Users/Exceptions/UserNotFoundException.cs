using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Application.Exceptions;

public sealed class UserNotFoundException : BusinessException
{
    public UserNotFoundException()
        : base(Domain.Resources.Message.UserNotFoundException)
    {
    }
}


public sealed class UserExistException : BusinessException
{
    public UserExistException()
        : base(Domain.Resources.Message.UserExistException)
    {
    }
}
