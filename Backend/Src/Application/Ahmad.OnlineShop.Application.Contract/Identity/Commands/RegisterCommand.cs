using AhmadBase.Application;

namespace Identity.Application.Commands;

public record RegisterCommand(
    string  FullName,
    string  Email,
    string  Password,
    string? PhoneNumber = null
) : ICommand<long>;
