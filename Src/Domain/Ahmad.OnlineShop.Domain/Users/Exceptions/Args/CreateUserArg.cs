namespace Ahmad.OnlineShop.Domain.Users.Args;

public sealed record CreateUserArg(
    long Id,
    string PhoneNumber
);
