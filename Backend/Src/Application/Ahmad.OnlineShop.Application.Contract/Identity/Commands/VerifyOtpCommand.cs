using AhmadBase.Application;

namespace Identity.Application.Commands;

/// <summary>تأیید کد OTP و ورود به سیستم</summary>
public record VerifyOtpCommand(
    string PhoneNumber,
    string Code
) : ICommand<LoginCommandResponse>;
