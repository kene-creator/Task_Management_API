
using Newtonsoft.Json;

namespace Task_Management.TokenDtos
{
    public record TokenDto
    {
        public required string AccessToken { get; init; }

        public required string RefreshToken { get; init; }
    }
}

