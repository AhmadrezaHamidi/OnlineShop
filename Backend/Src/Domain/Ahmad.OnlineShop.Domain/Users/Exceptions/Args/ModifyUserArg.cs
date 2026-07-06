namespace Ahmad.OnlineShop.Domain.Users.Args;

public sealed record ModifyUserArg(
    string Name,
    string Family,
    string? Email
);