using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Contract.Invoice.Commands;

/// <summary>
/// ایجاد فاکتور برای سفارش تأیید‌شده.
/// معمولاً از OrderConfirmedEventHandler صدا زده می‌شود (Observer Pattern).
/// </summary>
public record CreateInvoiceCommand(long OrderId) : ICommand<long>;
