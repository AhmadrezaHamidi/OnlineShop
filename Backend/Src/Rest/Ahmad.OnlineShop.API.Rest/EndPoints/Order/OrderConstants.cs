namespace Ahmad.OnlineShop.Rest.EndPoints.Order;

public static class OrderConstants
{
    public static class Routes
    {
        public const string BaseRoute = "api/v{version:apiVersion}/Order";

        public const string GetOrders       = "/";
        public const string GetOrder        = "/{id}";
        public const string CreateOrder     = "/";
        public const string AddItem         = "/{id}/Items";
        public const string RemoveItem      = "/{id}/Items/{itemId}";
        public const string PlaceOrder      = "/{id}/Place";
        public const string ConfirmOrder    = "/{id}/Confirm";
        public const string ShipOrder       = "/{id}/Ship";
        public const string DeliverOrder    = "/{id}/Deliver";
        public const string CancelOrder     = "/{id}/Cancel";

        public const string RecordPayment   = "/{id}/Payments";
        public const string CompletePayment = "/{id}/Payments/{paymentId}/Complete";
        public const string FailPayment     = "/{id}/Payments/{paymentId}/Fail";
    }

    public static class Names
    {
        public const string GetOrders       = "GetOrders";
        public const string GetOrder        = "GetOrder";
        public const string CreateOrder     = "CreateOrder";
        public const string AddItem         = "AddOrderItem";
        public const string RemoveItem      = "RemoveOrderItem";
        public const string PlaceOrder      = "PlaceOrder";
        public const string ConfirmOrder    = "ConfirmOrder";
        public const string ShipOrder       = "ShipOrder";
        public const string DeliverOrder    = "DeliverOrder";
        public const string CancelOrder     = "CancelOrder";
        public const string RecordPayment   = "RecordPayment";
        public const string CompletePayment = "CompletePayment";
        public const string FailPayment     = "FailPayment";
    }

    public static class Docs
    {
        public static class GetOrders
        {
            public const string Summary     = "دریافت لیست سفارش‌ها";
            public const string Description = "این سرویس لیست سفارش‌ها را با امکان فیلتر و صفحه‌بندی برمی‌گرداند";
        }
        public static class GetOrder
        {
            public const string Summary     = "دریافت جزئیات سفارش";
            public const string Description = "این سرویس اطلاعات کامل یک سفارش خاص را بر اساس شناسه برمی‌گرداند";
        }
        public static class CreateOrder
        {
            public const string Summary     = "ایجاد سفارش جدید";
            public const string Description = "این سرویس برای ثبت سفارش جدید استفاده می‌شود";
        }
        public static class AddItem
        {
            public const string Summary     = "افزودن آیتم به سفارش";
            public const string Description = "این سرویس یک محصول با تعداد مشخص به سفارش اضافه می‌کند";
        }
        public static class RemoveItem
        {
            public const string Summary     = "حذف آیتم از سفارش";
            public const string Description = "این سرویس یک آیتم خاص را از سفارش حذف می‌کند";
        }
        public static class PlaceOrder
        {
            public const string Summary     = "نهایی‌سازی سفارش";
            public const string Description = "این سرویس سفارش را از حالت پیش‌نویس به حالت ثبت‌شده تغییر می‌دهد";
        }
        public static class ConfirmOrder
        {
            public const string Summary     = "تایید سفارش";
            public const string Description = "این سرویس سفارش را توسط مدیر تایید می‌کند";
        }
        public static class ShipOrder
        {
            public const string Summary     = "ارسال سفارش";
            public const string Description = "این سرویس وضعیت سفارش را به در حال ارسال تغییر می‌دهد";
        }
        public static class DeliverOrder
        {
            public const string Summary     = "تحویل سفارش";
            public const string Description = "این سرویس تحویل سفارش به مشتری را ثبت می‌کند";
        }
        public static class CancelOrder
        {
            public const string Summary     = "لغو سفارش";
            public const string Description = "این سرویس سفارش را با ذکر دلیل لغو می‌کند";
        }
        public static class RecordPayment
        {
            public const string Summary     = "ثبت پرداخت";
            public const string Description = "این سرویس یک پرداخت جدید برای سفارش ثبت می‌کند";
        }
        public static class CompletePayment
        {
            public const string Summary     = "تایید پرداخت";
            public const string Description = "این سرویس پرداخت را به عنوان موفق ثبت می‌کند";
        }
        public static class FailPayment
        {
            public const string Summary     = "شکست پرداخت";
            public const string Description = "این سرویس پرداخت را به عنوان ناموفق ثبت می‌کند";
        }
    }
}
