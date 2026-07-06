/// <summary>
/// تست‌های فرآیند پرداخت سفارش
/// ─────────────────────────────────────────────────────────────────────
/// سناریو ۱: پرداخت آنلاین زرین‌پال — موفق
///   ثبت سفارش → درخواست زرین‌پال → redirect → Verify → تأیید سفارش
///
/// سناریو ۲: پرداخت آنلاین زرین‌پال — ناموفق
///   ثبت سفارش → verify شکست خورد → سفارش در Pending می‌ماند
///
/// سناریو ۳: پرداخت درب منزل
///   ثبت سفارش → سفارش بدون پرداخت آنلاین تأیید می‌شود
///
/// سناریو ۴: چرخه کامل سفارش
///   ثبت → پرداخت → تأیید → ارسال → تحویل
///
/// سناریو ۵: لغو سفارش
///   ثبت → لغو (قبل از ارسال) → موجودی آزاد می‌شود
/// </summary>
using Ahmad.OnlineShop.Domain.Order.Enums;
using Ahmad.OnlineShop.Domain.Order.Exceptions;
using OrderAgg = Ahmad.OnlineShop.Domain.Order.Aggregates;

namespace Ahmad.OnlineShop.Domain.Order.Tests;

public class OrderPaymentScenarioTests
{
    // ── Factory ──────────────────────────────────────────────────────────────

    private static OrderAgg.Order MakeOrder(PaymentMethod method = PaymentMethod.ZarinPal)
        => OrderAgg.Order.Create(new Ahmad.OnlineShop.Domain.Order.Args.CreateOrderArg(1, 100, method));

    private static OrderAgg.Order MakeOrderWithItems(PaymentMethod method = PaymentMethod.ZarinPal)
    {
        var order = MakeOrder(method);
        order.AddItem(new Ahmad.OnlineShop.Domain.Order.Args.AddOrderItemArg(1, 101, 2, 150_000m));
        return order;
    }

    // ═══════════════════════════════════════════════════════════════════════
    // سناریو ۱: پرداخت زرین‌پال موفق
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// گام ۱: سفارش ثبت می‌شود با روش ZarinPal
    /// گام ۲: پرداخت Record می‌شود (در انتظار redirect)
    /// گام ۳: کاربر پرداخت می‌کند و Verify موفق می‌شود
    /// گام ۴: سفارش Confirmed می‌شود
    /// </summary>
    [Fact]
    public void Scenario1_ZarinPal_Payment_Success_Should_Confirm_Order()
    {
        // ── گام ۱: ثبت سفارش با روش زرین‌پال
        var order = MakeOrderWithItems(PaymentMethod.ZarinPal);
        Assert.Equal(PaymentMethod.ZarinPal, order.PaymentMethod);
        Assert.Equal(OrderStatus.Pending,    order.Status);

        // ── گام ۲: شروع فرآیند پرداخت — Record کردن payment
        var payment = order.RecordPayment(
            new Ahmad.OnlineShop.Domain.Order.Args.RecordPaymentArg(
                PaymentId: 1001,
                Amount:    order.TotalAmount,
                Provider:  "ZarinPal"));

        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.Equal(order.TotalAmount,     payment.Amount);
        Assert.Single(order.Payments);

        // ── گام ۳: کاربر از درگاه برگشت — Verify موفق → MarkCompleted
        order.MarkPaymentCompleted(payment.Id);

        // ── گام ۴: سفارش باید Confirmed شود
        Assert.Equal(OrderStatus.Confirmed, order.Status);
        Assert.True(payment.IsSuccessful);
        Assert.NotNull(payment.PaidAt);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // سناریو ۲: پرداخت زرین‌پال ناموفق
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// گام ۱: سفارش ثبت، پرداخت Record
    /// گام ۲: Verify شکست خورد → MarkFailed
    /// گام ۳: سفارش همچنان Pending است
    /// </summary>
    [Fact]
    public void Scenario2_ZarinPal_Payment_Failed_Order_Stays_Pending()
    {
        var order = MakeOrderWithItems(PaymentMethod.ZarinPal);

        // ── ثبت payment
        var payment = order.RecordPayment(
            new Ahmad.OnlineShop.Domain.Order.Args.RecordPaymentArg(1001, order.TotalAmount, "ZarinPal"));

        // ── Verify ناموفق (کاربر پرداخت نکرد یا لغو کرد)
        order.MarkPaymentFailed(payment.Id);

        // ── سفارش باید Pending بماند (تأیید نشده)
        Assert.Equal(PaymentStatus.Failed,  payment.Status);
        Assert.Equal(OrderStatus.Pending,   order.Status);
        Assert.False(payment.IsSuccessful);

        // ── خطا: بدون پرداخت موفق نمی‌شود Confirm کرد
        Assert.Throws<OrderCannotConfirmWithoutPaymentException>(() => order.Confirm());
    }

    /// <summary>
    /// پرداخت دوباره بعد از شکست اول — باید موفق شود
    /// کاربر می‌تواند مجدداً تلاش کند
    /// </summary>
    [Fact]
    public void Scenario2b_Retry_Payment_After_Failed_Should_Succeed()
    {
        var order = MakeOrderWithItems(PaymentMethod.ZarinPal);

        // ── اول ناموفق
        var p1 = order.RecordPayment(
            new Ahmad.OnlineShop.Domain.Order.Args.RecordPaymentArg(1001, order.TotalAmount, "ZarinPal"));
        order.MarkPaymentFailed(p1.Id);
        Assert.Equal(OrderStatus.Pending, order.Status);

        // ── دوباره تلاش — این بار موفق
        var p2 = order.RecordPayment(
            new Ahmad.OnlineShop.Domain.Order.Args.RecordPaymentArg(1002, order.TotalAmount, "ZarinPal"));
        order.MarkPaymentCompleted(p2.Id);

        // ── حالا تأیید شود
        Assert.Equal(OrderStatus.Confirmed, order.Status);
        Assert.Equal(2, order.Payments.Count); // دو پرداخت ثبت شده
    }

    // ═══════════════════════════════════════════════════════════════════════
    // سناریو ۳: پرداخت درب منزل
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// پرداخت درب منزل: سفارش ثبت می‌شود — پرداخت هنگام تحویل
    /// سفارش با پرداخت درب منزل نیاز به Record payment آنلاین ندارد
    /// </summary>
    [Fact]
    public void Scenario3_CashOnDelivery_Order_Should_Confirm_With_Manual_Payment()
    {
        // ── سفارش با روش درب منزل
        var order = MakeOrderWithItems(PaymentMethod.CashOnDelivery);
        Assert.Equal(PaymentMethod.CashOnDelivery, order.PaymentMethod);

        // ── پرداخت نقدی درب منزل — به صورت دستی Record می‌شود
        var payment = order.RecordPayment(
            new Ahmad.OnlineShop.Domain.Order.Args.RecordPaymentArg(
                PaymentId: 2001,
                Amount:    order.TotalAmount,
                Provider:  "CashOnDelivery"));

        // ── تأیید تحویل نقدی
        order.MarkPaymentCompleted(payment.Id);

        Assert.Equal(OrderStatus.Confirmed, order.Status);
        Assert.True(payment.IsSuccessful);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // سناریو ۴: چرخه کامل سفارش زرین‌پال
    // ثبت → پرداخت → تأیید → ارسال → تحویل
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// فرآیند کامل یک سفارش زرین‌پال از ثبت تا تحویل
    /// </summary>
    [Fact]
    public void Scenario4_Full_Order_Lifecycle_ZarinPal()
    {
        // ── ثبت سفارش
        var order = MakeOrderWithItems(PaymentMethod.ZarinPal);
        Assert.Equal(OrderStatus.Pending, order.Status);

        // ── پرداخت موفق
        var payment = order.RecordPayment(
            new Ahmad.OnlineShop.Domain.Order.Args.RecordPaymentArg(1, order.TotalAmount, "ZarinPal"));
        order.MarkPaymentCompleted(payment.Id);
        Assert.Equal(OrderStatus.Confirmed, order.Status);

        // ── ارسال
        order.Ship();
        Assert.Equal(OrderStatus.Shipped, order.Status);

        // ── تحویل
        order.Deliver();
        Assert.Equal(OrderStatus.Delivered, order.Status);

        // ── تأیید رویدادها
        var events = order.DomainEvents.ToList();
        Assert.NotEmpty(events);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // سناریو ۵: لغو سفارش
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// سفارش قبل از ارسال می‌تواند لغو شود
    /// بعد از Delivered لغو امکانپذیر نیست
    /// </summary>
    [Fact]
    public void Scenario5_Cancel_Before_Ship_Should_Succeed()
    {
        var order = MakeOrderWithItems();

        // ── لغو قبل از ارسال
        order.Cancel("انصراف مشتری");

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    /// <summary>خطا: لغو سفارش Delivered → OrderAlreadyDeliveredException</summary>
    [Fact]
    public void Scenario5b_Cancel_After_Delivery_Should_Throw()
    {
        var order = MakeOrderWithItems();
        var pay   = order.RecordPayment(
            new Ahmad.OnlineShop.Domain.Order.Args.RecordPaymentArg(1, order.TotalAmount, null));
        order.MarkPaymentCompleted(pay.Id);
        order.Ship();
        order.Deliver();

        Assert.Throws<OrderAlreadyDeliveredException>(
            () => order.Cancel("دیر شد"));
    }

    // ═══════════════════════════════════════════════════════════════════════
    // تست‌های مقایسه روش پرداخت
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>سفارش با روش ZarinPal باید PaymentMethod درست داشته باشد</summary>
    [Fact]
    public void Order_PaymentMethod_ZarinPal_Should_Be_Set_Correctly()
    {
        var order = MakeOrder(PaymentMethod.ZarinPal);
        Assert.Equal(PaymentMethod.ZarinPal, order.PaymentMethod);
    }

    /// <summary>سفارش با روش CashOnDelivery باید PaymentMethod درست داشته باشد</summary>
    [Fact]
    public void Order_PaymentMethod_CashOnDelivery_Should_Be_Set_Correctly()
    {
        var order = MakeOrder(PaymentMethod.CashOnDelivery);
        Assert.Equal(PaymentMethod.CashOnDelivery, order.PaymentMethod);
    }

    /// <summary>مبلغ پرداخت باید با TotalAmount سفارش برابر باشد</summary>
    [Fact]
    public void Payment_Amount_Should_Match_Order_TotalAmount()
    {
        var order = MakeOrderWithItems();

        Assert.Throws<PaymentAmountMismatchException>(() =>
            order.RecordPayment(
                new Ahmad.OnlineShop.Domain.Order.Args.RecordPaymentArg(
                    1, order.TotalAmount + 1000, null)));
    }
}
