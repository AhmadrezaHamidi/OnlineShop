namespace Ahmad.OnlineShop.Rest.EndPoints.Identity;

public static class IdentityConstants
{
    public static class Routes
    {
        public const string BaseRoute = "api/v{version:apiVersion}/Identity";

        public const string GetUsers       = "/Users";
        public const string GetUser        = "/Users/{id}";
        public const string GetRoles       = "/Roles";
        public const string AssignRole     = "/Users/{id}/Roles/{roleId}";
        public const string RemoveRole     = "/Users/{id}/Roles/{roleId}";
        public const string ActivateUser   = "/Users/{id}/Activate";
        public const string DeactivateUser = "/Users/{id}/Deactivate";
        public const string SuspendUser    = "/Users/{id}/Suspend";
        public const string UpdateProfile  = "/Users/{id}/Profile";

        public const string Register       = "/Auth/Register";
        public const string Login          = "/Auth/Login";
        public const string Logout         = "/Auth/Logout";
        public const string RefreshToken   = "/Auth/RefreshToken";
        public const string ChangePassword = "/Auth/ChangePassword";
    }

    public static class Names
    {
        public const string GetUsers       = "GetUsers";
        public const string GetUser        = "GetUser";
        public const string GetRoles       = "GetRoles";
        public const string AssignRole     = "AssignRole";
        public const string RemoveRole     = "RemoveRole";
        public const string ActivateUser   = "ActivateUser";
        public const string DeactivateUser = "DeactivateUser";
        public const string SuspendUser    = "SuspendUser";
        public const string UpdateProfile  = "UpdateProfile";
        public const string Register       = "Register";
        public const string Login          = "Login";
        public const string Logout         = "Logout";
        public const string RefreshToken   = "RefreshToken";
        public const string ChangePassword = "ChangePassword";
    }

    public static class Docs
    {
        public static class GetUsers
        {
            public const string Summary     = "لیست کاربران";
            public const string Description = "این سرویس لیست کاربران را با امکان فیلتر و صفحه‌بندی برمی‌گرداند";
        }
        public static class GetUser
        {
            public const string Summary     = "دریافت اطلاعات کاربر";
            public const string Description = "این سرویس اطلاعات کامل یک کاربر را برمی‌گرداند";
        }
        public static class GetRoles
        {
            public const string Summary     = "لیست نقش‌ها";
            public const string Description = "این سرویس تمام نقش‌های موجود در سیستم را برمی‌گرداند";
        }
        public static class AssignRole
        {
            public const string Summary     = "اختصاص نقش به کاربر";
            public const string Description = "این سرویس یک نقش مشخص را به کاربر اختصاص می‌دهد";
        }
        public static class RemoveRole
        {
            public const string Summary     = "حذف نقش از کاربر";
            public const string Description = "این سرویس یک نقش مشخص را از کاربر حذف می‌کند";
        }
        public static class ActivateUser
        {
            public const string Summary     = "فعال‌سازی کاربر";
            public const string Description = "این سرویس حساب کاربر را فعال می‌کند";
        }
        public static class DeactivateUser
        {
            public const string Summary     = "غیرفعال‌سازی کاربر";
            public const string Description = "این سرویس حساب کاربر را غیرفعال می‌کند";
        }
        public static class SuspendUser
        {
            public const string Summary     = "تعلیق کاربر";
            public const string Description = "این سرویس حساب کاربر را به حالت تعلیق در می‌آورد";
        }
        public static class UpdateProfile
        {
            public const string Summary     = "ویرایش پروفایل";
            public const string Description = "این سرویس اطلاعات پروفایل کاربر را بروزرسانی می‌کند";
        }
        public static class Register
        {
            public const string Summary     = "ثبت‌نام";
            public const string Description = "این سرویس برای ثبت‌نام کاربر جدید استفاده می‌شود";
        }
        public static class Login
        {
            public const string Summary     = "ورود";
            public const string Description = "این سرویس برای احراز هویت و دریافت توکن استفاده می‌شود";
        }
        public static class Logout
        {
            public const string Summary     = "خروج";
            public const string Description = "این سرویس برای خروج از سیستم و باطل کردن Session استفاده می‌شود";
        }
        public static class RefreshToken
        {
            public const string Summary     = "تمدید توکن";
            public const string Description = "این سرویس توکن جدید با استفاده از Refresh Token صادر می‌کند";
        }
        public static class ChangePassword
        {
            public const string Summary     = "تغییر رمز عبور";
            public const string Description = "این سرویس رمز عبور کاربر جاری را تغییر می‌دهد";
        }
    }
}
