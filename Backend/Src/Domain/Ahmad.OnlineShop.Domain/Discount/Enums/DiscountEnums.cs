namespace Ahmad.OnlineShop.Domain.Discount.Enums;

public enum DiscountType
{
    Percentage,   // درصدی — مثلاً ۲۰٪
    FixedAmount   // مبلغ ثابت — مثلاً ۵۰,۰۰۰ تومان
}

public enum DiscountStatus
{
    Active,
    Inactive,
    Expired     // منقضی‌شده (ExpiresAt گذشته)
}
