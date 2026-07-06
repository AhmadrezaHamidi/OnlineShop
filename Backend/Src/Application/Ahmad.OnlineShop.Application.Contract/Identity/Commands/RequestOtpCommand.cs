using AhmadBase.Application;

namespace Identity.Application.Commands;

/// <summary>درخواست ارسال کد OTP به شماره موبایل</summary>
public record RequestOtpCommand(string PhoneNumber) : ICommand<bool>;
