using AhmadBase.Application;

namespace Identity.Application.Commands;

public record UpdateProfileCommand(
    long    UserId,
    string  FullName,
    string? PhoneNumber = null
) : ICommand<bool>;
