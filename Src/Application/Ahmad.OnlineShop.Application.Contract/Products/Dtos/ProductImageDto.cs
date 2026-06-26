using Ahmad.OnlineShop.Domain.Enums;

namespace Ahmad.OnlineShop.Application.Dtos;

public record ProductImageDto(
    Guid      Id,
    string    Url,
    ImageType Type,
    int       SortOrder,
    DateTime  UploadedAt
);
