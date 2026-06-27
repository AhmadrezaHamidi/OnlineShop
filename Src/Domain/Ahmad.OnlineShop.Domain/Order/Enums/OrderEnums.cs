namespace Ahmad.OnlineShop.Domain.Order.Enums;

public enum OrderStatus
{
    Pending,        // ثبت شده، منتظر پرداخت
    Confirmed,      // پرداخت تأیید شد
    Processing,     // در حال آماده‌سازی
    Shipped,        // ارسال شد
    Delivered,      // تحویل داده شد
    Cancelled,      // لغو شد
    Refunded        // مسترد شد
}

public enum PaymentStatus
{
    Pending,        // در انتظار
    Completed,      // موفق
    Failed,         // ناموفق
    Refunded        // مسترد
}

public enum PaymentMethod
{
    ZarinPal,       // پرداخت آنلاین از طریق زرین‌پال
    BNPL,           // پرداخت اقساطی (Buy Now Pay Later)
    Wallet,         // کیف پول
    CashOnDelivery  // پرداخت درب منزل
}
