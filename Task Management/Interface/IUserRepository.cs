using Task_Management.Entities;
using Task_Management.TokenDtos;

namespace Task_Management.Interface
{
    public interface IUserRepository
    {
        Task<User> RegisterUserAsync(User user);
        Task<User> GetUserByEmailAsync(string email);

        Task<User> GetUserByIdAsync(Guid id);
        bool VerifyPassword(string enteredPassword, string passwordHash);
        (string AccessToken, string RefreshToken) GenerateTokens(User user);
        Task<User> GetUserByRefreshTokenAsync(string refreshToken);
        bool ValidateRefreshToken(string refreshToken);
        Task<IEnumerable<User>> GetAllUsersAsync();

        string HashPassword(string password);
    }
}