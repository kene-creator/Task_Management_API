using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Task_Management.Data;
using Task_Management.Entities;
using Task_Management.Interface;
using Task_Management.TokenDtos;
using Task_Management.Utility;

namespace Task_Management.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly TaskManagementDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRepository(TaskManagementDbContext context, IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task<User> RegisterUserAsync(User user)
        {

            string insertQuery = @"
        INSERT INTO [TaskManagementDb].dbo.[Users] (UserId, CreatedAt, Email, FirstName, LastName, PasswordHash) VALUES (@UserId, @CreatedAt, @Email, @FirstName, @LastName, @PasswordHash);
    ";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId", user.Id);
                    command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@FirstName", user.FirstName);
                    command.Parameters.AddWithValue("@LastName", user.LastName);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    await command.ExecuteNonQueryAsync();
                }
            }

            return user;
        }

        public bool VerifyPassword(string providedPassword, string actualPasswordHash)
        {
            return BCrypt.Net.BCrypt.Verify(providedPassword, actualPasswordHash);
        }

        public string HashPassword(string password)
        {
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);
            return hashedPassword;
        }

        public (string AccessToken, string RefreshToken) GenerateTokens(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var issuer = _configuration.GetSection("Jwt:Issuer").Value;

            var accessToken = new JwtSecurityToken(issuer,
                issuer,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            var refreshToken = GenerateRefreshToken();
            return (
                AccessToken: new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken: refreshToken
            );
        }

        private string GenerateRefreshToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:RefreshTokenKey").Value));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddDays(30),
                Issuer = _configuration.GetSection("Jwt:Issuer").Value,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public void SetRefreshToken(RefreshToken refreshToken)
        {
            var response = _httpContextAccessor.HttpContext.Response;
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.Expires,
                Path = "/"
            };
            response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }

        public bool ValidateRefreshToken(string refreshToken)
        {
            try
            {
                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:RefreshTokenKey").Value));
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = _configuration.GetSection("Jwt:Issuer").Value,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(refreshToken, validationParameters, out validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
