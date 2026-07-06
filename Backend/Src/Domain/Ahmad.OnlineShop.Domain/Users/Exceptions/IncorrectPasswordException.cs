using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Users.Exceptions;

public sealed class IncorrectPasswordException : BusinessException
{
    public IncorrectPasswordException()
        : base(Resources.Message.IncorrectPasswordException)
    {
    }
}
