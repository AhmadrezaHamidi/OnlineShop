using System;
using AhmadBase.Application;


namespace Ahmad.OnlineShop.Application.Contract.User;

public sealed record RegisterCommand(
    string UserName,
    string Password,
    string ConfirmPassword,
    string? FirstName,
    string? LastName,
    string? Email,
    string? MobileNumber
) : ICommand<long>;
