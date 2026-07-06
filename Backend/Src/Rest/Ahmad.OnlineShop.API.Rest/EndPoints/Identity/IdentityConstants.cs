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

        public const string RequestOtp   = "/Auth/RequestOtp";
        public const string VerifyOtp    = "/Auth/VerifyOtp";
        public const string Logout       = "/Auth/Logout";
        public const string RefreshToken = "/Auth/RefreshToken";
    }

    public static class Names
    {
        private const string Prefix = "Identity_";
        public const string GetUsers       = Prefix + "GetUsers";
        public const string GetUser        = Prefix + "GetUser";
        public const string GetRoles       = Prefix + "GetRoles";
        public const string AssignRole     = Prefix + "AssignRole";
        public const string RemoveRole     = Prefix + "RemoveRole";
        public const string ActivateUser   = Prefix + "ActivateUser";
        public const string DeactivateUser = Prefix + "DeactivateUser";
        public const string SuspendUser    = Prefix + "SuspendUser";
        public const string UpdateProfile  = Prefix + "UpdateProfile";
        public const string RequestOtp     = Prefix + "RequestOtp";
        public const string VerifyOtp      = Prefix + "VerifyOtp";
        public const string Logout         = Prefix + "Logout";
        public const string RefreshToken   = Prefix + "RefreshToken";
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
        public static class RequestOtp
        {
            public const string Summary     = "درخواست کد تأیید";
            public const string Description = "کد OTP 6 رقمی به شماره موبایل ارسال می‌شود";
        }
        public static class VerifyOtp
        {
            public const string Summary     = "تأیید کد و ورود به سیستم";
            public const string Description = "پس از تأیید OTP، JWT و Refresh Token صادر می‌شود";
        }
        public static class Logout
        {
            public const string Summary     = "خروج از سیستم";
            public const string Description = "Refresh Token باطل می‌شود";
        }
        public static class RefreshToken
        {
            public const string Summary     = "تمدید توکن";
            public const string Description = "این سرویس توکن جدید با استفاده از Refresh Token صادر می‌کند";
        }
    }
}
