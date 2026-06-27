using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Application.Exceptions.BackOffice;

internal sealed class EmailExist : BusinessException
{
    public EmailExist() : base("ادمین با این ایمیل از قبل وجود دارد.") { }
}
