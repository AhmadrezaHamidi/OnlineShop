using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmad.OnlineShop.API.Rest.EndPoints.Account;

public static class AccountConstants
{
    // Routes
    public static class Routes
    {
        public const string BaseRoute = "api/v{version:apiVersion}/Account";

        public const string GetAccounts = "/";
        public const string GetAccountConditions = "/{accountNo}/Conditions";

        public const string Register = "/Register";
        public const string Login = "/Login";
        public const string RefreshToken = "/RefreshToken";
        public const string Logout = "/Logout";

        public const string GetProfile = "/Profile";
        public const string UpdateProfile = "/Profile";

        public const string ChangePassword = "/ChangePassword";
    }

    // Endpoint Names
    public static class Names
    {
        public const string GetAccounts = "GetAccounts";
        public const string GetAccountConditions = "GetAccountConditions";

        
        public const string Register = "RegisterAccount";
        public const string Login = "LoginAccount";
        public const string RefreshToken = "RefreshToken";
        public const string Logout = "Logout";

        public const string GetProfile = "GetProfile";
        public const string UpdateProfile = "UpdateProfile";

        public const string ChangePassword = "ChangePassword";
    }

    // Documentation
    public static class Docs
    {
        public static class GetAccounts
        {
            public const string Summary = "لیست حساب های کاربر";
            public const string Description = "این سرویس برای گرفتن لیست حساب های کاربر مورد استفاده قرار میگیرد";
        }

        public static class GetAccountConditions
        {
            public const string Summary = "بازبینی شرایط برداشت";
            public const string Description = "این سرویس برای گرفتن لیست امضا داران یک چک بررسی میشود";
        }

        public static class Register
        {
            public const string Summary = "ثبت نام کاربر";
            public const string Description = "این سرویس برای ثبت نام کاربر جدید در سیستم استفاده میشود";
        }

        public static class Login
        {
            public const string Summary = "ورود کاربر";
            public const string Description = "این سرویس برای احراز هویت کاربر و دریافت توکن استفاده میشود";
        }

        public static class RefreshToken
        {
            public const string Summary = "تمدید توکن";
            public const string Description = "این سرویس برای دریافت توکن جدید با استفاده از رفرش توکن استفاده میشود";
        }

        public static class Logout
        {
            public const string Summary = "خروج کاربر";
            public const string Description = "این سرویس برای خروج کاربر از سیستم و باطل کردن توکن استفاده میشود";
        }

        public static class GetProfile
        {
            public const string Summary = "دریافت پروفایل کاربر";
            public const string Description = "این سرویس اطلاعات پروفایل کاربر لاگین شده را برمیگرداند";
        }

        public static class UpdateProfile
        {
            public const string Summary = "ویرایش پروفایل";
            public const string Description = "این سرویس برای بروزرسانی اطلاعات پروفایل کاربر استفاده میشود";
        }

        public static class ChangePassword
        {
            public const string Summary = "تغییر رمز عبور";
            public const string Description = "این سرویس برای تغییر رمز عبور کاربر استفاده میشود";
        }
    }
}
