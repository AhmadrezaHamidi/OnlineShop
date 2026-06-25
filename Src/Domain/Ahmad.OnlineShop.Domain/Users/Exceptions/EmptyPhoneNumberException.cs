using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Users.Exceptions
{
    public sealed class EmptyPhoneNumberException : BusinessException
    {
        public EmptyPhoneNumberException()
            : base(Resources.Message.EmptyPhoneNumberException)
        {
        }
    }

}
