using Ahmad.OnlineShop.Domain.Products.Enums;
using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Ahmad.OnlineShop.Application.Commands;

public record AddProductImageCommand(
    [property: JsonIgnore] long ProductId,
    string    Url,
    string    BucketKey,
    ImageType Type
) : ICommand<long>;
