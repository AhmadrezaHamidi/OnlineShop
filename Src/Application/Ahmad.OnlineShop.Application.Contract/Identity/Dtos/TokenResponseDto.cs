namespace Identity.Application.Dtos;

public sealed record TokenResponseDto(
    string   AccessToken,
    string   RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt,
    long     UserId,
    string   FullName,
    string   Email
);
