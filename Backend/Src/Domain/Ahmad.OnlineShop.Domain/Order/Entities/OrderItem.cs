using Ahmad.OnlineShop.Domain.Order.Exceptions;
using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Order.Entities;

public sealed class OrderItem : TEntity<long>
{
    public long OrderId { get; private set; }
    public long ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice => Quantity * UnitPrice;

    private OrderItem() { }

    internal OrderItem(long id, long orderId, long productId, int quantity, decimal unitPrice)
    {
        GuardQuantity(quantity);
        GuardUnitPrice(unitPrice);

        Id = id;
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    internal void UpdateQuantity(int newQuantity)
    {
        GuardQuantity(newQuantity);
        Quantity = newQuantity;
    }

    private static void GuardQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new OrderItemInvalidQuantityException();
    }

    private static void GuardUnitPrice(decimal unitPrice)
    {
        if (unitPrice <= 0)
            throw new OrderItemInvalidUnitPriceException();
    }
}
