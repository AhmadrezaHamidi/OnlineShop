using AhmadBase.Application;
using Ahmad.OnlineShop.Domain.Enums;

namespace Ahmad.OnlineShop.Application.Commands;

public record AddProductImageCommand(
    long      ProductId,
    string    Url,
    string    BucketKey,
    ImageType Type
) : ICommand<long>;
