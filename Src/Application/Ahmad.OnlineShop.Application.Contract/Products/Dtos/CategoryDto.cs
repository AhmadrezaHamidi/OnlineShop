namespace Ahmad.OnlineShop.Application.Dtos;

public record CategoryDto(
    long  Id,
    string Name,
    long?  ParentId
);
