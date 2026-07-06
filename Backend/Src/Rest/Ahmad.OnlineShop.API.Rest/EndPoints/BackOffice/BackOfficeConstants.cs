namespace Ahmad.OnlineShop.Rest.EndPoints.BackOffice;

public static class BackOfficeConstants
{
    public static class Routes
    {
        public const string BaseRoute = "api/v{version:apiVersion}/BackOffice";

        public const string GetAdmins        = "/Admins";
        public const string GetAdmin         = "/Admins/{id}";
        public const string CreateAdmin      = "/Admins";
        public const string ActivateAdmin    = "/Admins/{id}/Activate";
        public const string DeactivateAdmin  = "/Admins/{id}/Deactivate";
        public const string SuspendAdmin     = "/Admins/{id}/Suspend";
        public const string ChangeAdminRole  = "/Admins/{id}/Role";
        public const string GetAuditLogs     = "/Admins/{id}/AuditLogs";

        public const string GetReports       = "/Reports";
        public const string RequestReport    = "/Reports";
        public const string CompleteReport   = "/Reports/{id}/Complete";
        public const string FailReport       = "/Reports/{id}/Fail";
    }

    public static class Names
    {
        public const string GetAdmins       = "GetAdmins";
        public const string GetAdmin        = "GetAdmin";
        public const string CreateAdmin     = "CreateAdmin";
        public const string ActivateAdmin   = "ActivateAdmin";
        public const string DeactivateAdmin = "DeactivateAdmin";
        public const string SuspendAdmin    = "SuspendAdmin";
        public const string ChangeAdminRole = "ChangeAdminRole";
        public const string GetAuditLogs    = "GetAuditLogs";
        public const string GetReports      = "GetReports";
        public const string RequestReport   = "RequestReport";
        public const string CompleteReport  = "CompleteReport";
        public const string FailReport      = "FailReport";
    }

    public static class Docs
    {
        public static class GetAdmins
        {
            public const string Summary     = "لیست مدیران";
            public const string Description = "این سرویس لیست کاربران مدیریتی را با امکان فیلتر برمی‌گرداند";
        }
        public static class GetAdmin
        {
            public const string Summary     = "دریافت اطلاعات مدیر";
            public const string Description = "این سرویس اطلاعات کامل یک مدیر را برمی‌گرداند";
        }
        public static class CreateAdmin
        {
            public const string Summary     = "ایجاد کاربر مدیریتی";
            public const string Description = "این سرویس برای ثبت کاربر مدیریتی جدید در سیستم استفاده می‌شود";
        }
        public static class ActivateAdmin
        {
            public const string Summary     = "فعال‌سازی مدیر";
            public const string Description = "این سرویس حساب کاربر مدیریتی را فعال می‌کند";
        }
        public static class DeactivateAdmin
        {
            public const string Summary     = "غیرفعال‌سازی مدیر";
            public const string Description = "این سرویس حساب کاربر مدیریتی را غیرفعال می‌کند";
        }
        public static class SuspendAdmin
        {
            public const string Summary     = "تعلیق مدیر";
            public const string Description = "این سرویس حساب کاربر مدیریتی را به حالت تعلیق در می‌آورد";
        }
        public static class ChangeAdminRole
        {
            public const string Summary     = "تغییر نقش مدیر";
            public const string Description = "این سرویس نقش کاربر مدیریتی را تغییر می‌دهد";
        }
        public static class GetAuditLogs
        {
            public const string Summary     = "دریافت لاگ‌های حسابرسی";
            public const string Description = "این سرویس تاریخچه اقدامات یک مدیر را برمی‌گرداند";
        }
        public static class GetReports
        {
            public const string Summary     = "لیست گزارش‌ها";
            public const string Description = "این سرویس لیست گزارش‌های درخواست‌شده را برمی‌گرداند";
        }
        public static class RequestReport
        {
            public const string Summary     = "درخواست گزارش";
            public const string Description = "این سرویس درخواست تولید گزارش جدید را ثبت می‌کند";
        }
        public static class CompleteReport
        {
            public const string Summary     = "تکمیل گزارش";
            public const string Description = "این سرویس گزارش را به عنوان آماده ثبت می‌کند";
        }
        public static class FailReport
        {
            public const string Summary     = "شکست گزارش";
            public const string Description = "این سرویس خطای تولید گزارش را ثبت می‌کند";
        }
    }
}
