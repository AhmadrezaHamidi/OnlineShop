using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmad.OnlineShop.Domain.User;

public class UserRole : IdentityUserRole<long>
{
    private UserRole() { }

    private UserRole(long roleId, long userId)
    {
        RoleId = roleId;
        UserId = userId;
    }

    public static UserRole New(long roleId, long userId)
    {
        return new UserRole(roleId, userId);
    }
}
