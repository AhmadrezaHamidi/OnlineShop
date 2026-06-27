using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Identity.Application.Commands;

public record UpdateProfileCommand(
    [property: JsonIgnore] long    UserId,
    string                         FullName,
    string?                        PhoneNumber = null
) : ICommand<bool>;
