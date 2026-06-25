using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Users.Exceptions;

public sealed class InvalidPhoneNumberException : BusinessException
{
    public InvalidPhoneNumberException()
        : base(Resources.Message.InvalidPhoneNumberException)
    {
    }
}
