using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task_Management.Entities;
using Task_Management.Interface;
using Task_Management.UserDtos;
using System;
using System.Threading.Tasks;
using Task_Management.Dtos.UserDtos;
using Task_Management.Enums;
using Task_Management.TokenDtos;
using Task_Management;

namespace Catalog_API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerManager _logger;

        public AuthController(IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [HttpPost("register/user")]
        public async Task<ActionResult<UserDto>> RegisterUserAsync([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = createUserDto.Email,
                    PasswordHash = _userRepository.HashPassword(createUserDto.Password),
                    CreatedAt = DateTimeOffset.UtcNow,
                    FirstName = createUserDto.FirstName,
                    LastName = createUserDto.LastName,
                };
                var registeredUser = await _userRepository.RegisterUserAsync(user);

                if (registeredUser == null)
                {
                    return BadRequest("Failed to register user");
                }

                return CreatedAtAction(nameof(RegisterUserAsync), new { id = user.Id }, user.AsUserDto());
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while registering user: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenDto>> LoginAsync([FromBody] LoginUserDto loginUserDto)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(loginUserDto.Email);

                if (user == null || !_userRepository.VerifyPassword(loginUserDto.Password, user.PasswordHash))
                {
                    return BadRequest("Email or password incorrect");
                }

                var token = _userRepository.GenerateTokens(user);

                return Ok(new TokenDto
                {
                    AccessToken = token.AccessToken,
                    RefreshToken = token.RefreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while logging in: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("user/{guid:guid}")]
        public async Task<ActionResult<UserDto>> GetUserAsync(Guid guid)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(guid);

                if (user == null)
                {
                    return NotFound();
                }

                return user.AsUserDto();

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting user from database: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("userByEmail")]
        public async Task<ActionResult<UserDto>> GetUserByEmailAsync([FromQuery] string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            return user.AsUserDto();
        }


        [HttpGet("users")]
        public async Task<ActionResult<UserDto>> GetUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving users: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("refresh-token")]
        public async Task<ActionResult<TokenDto>> RefreshTokenAsync()
        {
            var request = _httpContextAccessor.HttpContext?.Request.Cookies;
            var refreshToken = request?["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest("Refresh token not found in cookies");
            }

            if (!_userRepository.ValidateRefreshToken(refreshToken))
            {
                return BadRequest("Invalid refresh token");
            }

            var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);
            if (user == null)
            {
                return BadRequest("User not found for the given refresh token");
            }

            var newTokens = _userRepository.GenerateTokens(user);

            return Ok(new TokenDto
            {
                AccessToken = newTokens.AccessToken,
                RefreshToken = newTokens.RefreshToken
            });
        }
    }
}
