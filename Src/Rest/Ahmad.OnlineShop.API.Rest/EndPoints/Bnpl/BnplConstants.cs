namespace Ahmad.OnlineShop.Rest.EndPoints.Bnpl;

public static class BnplConstants
{
    public static class Routes
    {
        public const string BaseRoute = "api/v{version:apiVersion}/Bnpl";

        public const string CreateContract     = "/Contracts";
        public const string GetContract        = "/Contracts/{id}";
        public const string GetUserContracts   = "/Users/{userId}/Contracts";
        public const string GetInstallments    = "/Contracts/{id}/Installments";
        public const string PayInstallment     = "/Contracts/{id}/Installments/{installmentId}/Pay";
        public const string DefaultContract    = "/Contracts/{id}/Default";
        public const string CancelContract     = "/Contracts/{id}/Cancel";

        public const string GetCreditLimit     = "/Users/{userId}/Credit";
        public const string IncreaseCreditLimit= "/Users/{userId}/Credit/Increase";
        public const string BlockCredit        = "/Users/{userId}/Credit/Block";
        public const string ReleaseCredit      = "/Users/{userId}/Credit/Release";
    }

    public static class Names
    {
        public const string CreateContract      = "CreateBnplContract";
        public const string GetContract         = "GetBnplContract";
        public const string GetUserContracts    = "GetUserBnplContracts";
        public const string GetInstallments     = "GetInstallments";
        public const string PayInstallment      = "PayInstallment";
        public const string DefaultContract     = "DefaultBnplContract";
        public const string CancelContract      = "CancelBnplContract";
        public const string GetCreditLimit      = "GetCreditLimit";
        public const string IncreaseCreditLimit = "IncreaseCreditLimit";
        public const string BlockCredit         = "BlockCredit";
        public const string ReleaseCredit       = "ReleaseCredit";
    }

    public static class Docs
    {
        public static class CreateContract
        {
            public const string Summary     = "ایجاد قرارداد BNPL";
            public const string Description = "این سرویس قرارداد خرید اقساطی جدید برای یک سفارش ایجاد می‌کند";
        }
        public static class GetContract
        {
            public const string Summary     = "دریافت جزئیات قرارداد";
            public const string Description = "این سرویس اطلاعات کامل یک قرارداد اقساطی را برمی‌گرداند";
        }
        public static class GetUserContracts
        {
            public const string Summary     = "لیست قراردادهای کاربر";
            public const string Description = "این سرویس تمام قراردادهای اقساطی یک کاربر را برمی‌گرداند";
        }
        public static class GetInstallments
        {
            public const string Summary     = "لیست اقساط قرارداد";
            public const string Description = "این سرویس تمام اقساط یک قرارداد را با وضعیت هر قسط برمی‌گرداند";
        }
        public static class PayInstallment
        {
            public const string Summary     = "پرداخت قسط";
            public const string Description = "این سرویس پرداخت یک قسط مشخص از قرارداد را ثبت می‌کند";
        }
        public static class DefaultContract
        {
            public const string Summary     = "ثبت نکول قرارداد";
            public const string Description = "این سرویس قرارداد را به حالت نکول تغییر می‌دهد";
        }
        public static class CancelContract
        {
            public const string Summary     = "لغو قرارداد";
            public const string Description = "این سرویس قرارداد اقساطی را لغو و سقف اعتبار را آزاد می‌کند";
        }
        public static class GetCreditLimit
        {
            public const string Summary     = "دریافت سقف اعتبار";
            public const string Description = "این سرویس سقف اعتبار BNPL کاربر را برمی‌گرداند";
        }
        public static class IncreaseCreditLimit
        {
            public const string Summary     = "افزایش سقف اعتبار";
            public const string Description = "این سرویس سقف اعتبار BNPL کاربر را افزایش می‌دهد";
        }
        public static class BlockCredit
        {
            public const string Summary     = "مسدود کردن اعتبار";
            public const string Description = "این سرویس بخشی از اعتبار کاربر را مسدود می‌کند";
        }
        public static class ReleaseCredit
        {
            public const string Summary     = "آزادسازی اعتبار";
            public const string Description = "این سرویس اعتبار مسدود‌شده را آزاد می‌کند";
        }
    }
}
