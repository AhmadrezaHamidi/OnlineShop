using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Users.Exceptions;

public sealed class InvalidNationalCodeException : BusinessException
{
    public InvalidNationalCodeException()
        : base(Resources.Message.InvalidNationalCodeException)
    {
    }
}
