using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Application.Exceptions;

public sealed class PasswordMismatchException : BusinessException
{
    public PasswordMismatchException()
        : base(Domain.Resources.Message.PasswordMismatchException)
    {
    }
}