using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Order.Exceptions;

// Order
public sealed class OrderNotFoundException : BusinessException
{
    public OrderNotFoundException() : base("سفارش مورد نظر یافت نشد.") { }
}

public sealed class OrderNoItemsException : BusinessException
{
    public OrderNoItemsException() : base("سفارش باید حداقل یک آیتم داشته باشد.") { }
}

public sealed class OrderAlreadyCancelledException : BusinessException
{
    public OrderAlreadyCancelledException() : base("سفارش قبلاً لغو شده است.") { }
}

public sealed class OrderAlreadyDeliveredException : BusinessException
{
    public OrderAlreadyDeliveredException() : base("سفارش قبلاً تحویل داده شده است.") { }
}

public sealed class OrderInvalidStatusTransitionException : BusinessException
{
    public OrderInvalidStatusTransitionException() : base("تغییر وضعیت سفارش در حالت فعلی مجاز نیست.") { }
}

// OrderItem
public sealed class OrderItemNotFoundException : BusinessException
{
    public OrderItemNotFoundException() : base("آیتم سفارش یافت نشد.") { }
}

public sealed class OrderItemInvalidQuantityException : BusinessException
{
    public OrderItemInvalidQuantityException() : base("تعداد آیتم باید بزرگتر از صفر باشد.") { }
}

public sealed class OrderItemInvalidUnitPriceException : BusinessException
{
    public OrderItemInvalidUnitPriceException() : base("قیمت واحد باید بزرگتر از صفر باشد.") { }
}

// Payment
public sealed class PaymentNotFoundException : BusinessException
{
    public PaymentNotFoundException() : base("پرداخت مورد نظر یافت نشد.") { }
}

public sealed class PaymentAmountMismatchException : BusinessException
{
    public PaymentAmountMismatchException() : base("مبلغ پرداخت با مجموع سفارش مطابقت ندارد.") { }
}

public sealed class PaymentInvalidAmountException : BusinessException
{
    public PaymentInvalidAmountException() : base("مبلغ پرداخت نامعتبر است.") { }
}

public sealed class OrderCannotConfirmWithoutPaymentException : BusinessException
{
    public OrderCannotConfirmWithoutPaymentException() : base("سفارش بدون پرداخت موفق قابل تأیید نیست.") { }
}
