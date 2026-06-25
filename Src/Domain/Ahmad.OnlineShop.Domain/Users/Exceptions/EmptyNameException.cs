using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Users.Exceptions
{
    public sealed class EmptyNameException : BusinessException
    {
        public EmptyNameException()
            : base(Resources.Message.EmptyNameException)
        {
        }
    }

}
