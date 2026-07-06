using AhmadBase.Doamin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmad.OnlineShop.Domain.Users.Exceptions;

public sealed class ExistingUserException : BusinessException
{
    public ExistingUserException()
        : base(Resources.Message.ExistingUserException)
    {
    }
}
