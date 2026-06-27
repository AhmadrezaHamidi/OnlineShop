using AhmadBase.Doamin;

namespace Identity.Domain.Exceptions;

public sealed class UserNotFoundException : BusinessException
{
    public UserNotFoundException() : base(Ahmad.OnlineShop.Domain.Resources.Message.UserNotFoundException) { }
}

public sealed class UserAlreadyExistsException : BusinessException
{
    public UserAlreadyExistsException() : base(Ahmad.OnlineShop.Domain.Resources.Message.UserExistException) { }
}

public sealed class IncorrectPasswordException : BusinessException
{
    public IncorrectPasswordException() : base(Ahmad.OnlineShop.Domain.Resources.Message.IncorrectPasswordException) { }
}

public sealed class RoleNotFoundException : BusinessException
{
    public RoleNotFoundException() : base(Ahmad.OnlineShop.Domain.Resources.Message.RoleNotFoundException) { }
}

public sealed class InvalidRefreshTokenException : BusinessException
{
    public InvalidRefreshTokenException() : base(Ahmad.OnlineShop.Domain.Resources.Message.InvalidRefreshTokenException) { }
}
