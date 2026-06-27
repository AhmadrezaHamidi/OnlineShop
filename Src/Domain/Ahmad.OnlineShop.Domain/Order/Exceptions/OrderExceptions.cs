using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Order.Exceptions;

// Order
public sealed class OrderNotFoundException : BusinessException
{
    public OrderNotFoundException() : base(Resources.Message.OrderNotFoundException) { }
}

public sealed class OrderNoItemsException : BusinessException
{
    public OrderNoItemsException() : base(Resources.Message.OrderNoItemsException) { }
}

public sealed class OrderAlreadyCancelledException : BusinessException
{
    public OrderAlreadyCancelledException() : base(Resources.Message.OrderAlreadyCancelledException) { }
}

public sealed class OrderAlreadyDeliveredException : BusinessException
{
    public OrderAlreadyDeliveredException() : base(Resources.Message.OrderAlreadyDeliveredException) { }
}

public sealed class OrderInvalidStatusTransitionException : BusinessException
{
    public OrderInvalidStatusTransitionException() : base(Resources.Message.OrderInvalidStatusTransitionException) { }
}

// OrderItem
public sealed class OrderItemNotFoundException : BusinessException
{
    public OrderItemNotFoundException() : base(Resources.Message.OrderItemNotFoundException) { }
}

public sealed class OrderItemInvalidQuantityException : BusinessException
{
    public OrderItemInvalidQuantityException() : base(Resources.Message.OrderItemInvalidQuantityException) { }
}

public sealed class OrderItemInvalidUnitPriceException : BusinessException
{
    public OrderItemInvalidUnitPriceException() : base(Resources.Message.OrderItemInvalidUnitPriceException) { }
}

// Payment
public sealed class PaymentNotFoundException : BusinessException
{
    public PaymentNotFoundException() : base(Resources.Message.PaymentNotFoundException) { }
}

public sealed class PaymentAmountMismatchException : BusinessException
{
    public PaymentAmountMismatchException() : base(Resources.Message.PaymentAmountMismatchException) { }
}

public sealed class PaymentInvalidAmountException : BusinessException
{
    public PaymentInvalidAmountException() : base(Resources.Message.PaymentInvalidAmountException) { }
}

public sealed class OrderCannotConfirmWithoutPaymentException : BusinessException
{
    public OrderCannotConfirmWithoutPaymentException() : base(Resources.Message.OrderCannotConfirmWithoutPaymentException) { }
}
