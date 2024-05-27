namespace Task_Management.Dtos.TokenDtos;

public record RefreshTokenRequestDto
{
    public required string RefreshToken { get; set; }
}