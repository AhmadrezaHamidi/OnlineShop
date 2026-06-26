namespace Ahmad.OnlineShop.Domain.Order.Enums;

public enum OrderStatus
{
    Pending,     // ثبت شده، منتظر پرداخت
    Confirmed,   // پرداخت تأیید شد
    Processing,  // در حال آماده‌سازی
    Shipped,     // ارسال شد
    Delivered,   // تحویل داده شد
    Cancelled,   // لغو شد
    Refunded     // مسترد شد
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Refunded
}

public enum PaymentMethod
{
    Online,
    BNPL,
    Wallet,
    CashOnDelivery
}
