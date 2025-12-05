using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShopPrimeVueServerAPI.Models;
using ShopPrimeVueServerAPI.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShopPrimeVueServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUsersService _usersService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IConfiguration config,
            IUsersService usersService,
            ILogger<AuthController> logger)
        {
            _config = config;
            _usersService = usersService;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Некорректные данные", errors = ModelState });

                var existingUser = await _usersService.GetUserByEmailAsync(request.Email);
                if (existingUser != null)
                    return BadRequest(new { message = "Пользователь с таким email уже существует" });

                var user = new Users
                {
                    Email = request.Email,
                    Password = request.Password,
                    UsersName = request.UsersName,
                    Role = "user",
                    isAppruved = true
                };

                var createdUser = await _usersService.RegisterUserAsync(user);

                var token = GenerateJwtToken(createdUser);
                SetAuthCookie(token);

                _logger.LogInformation("Пользователь зарегистрирован: {Email}", createdUser.Email);
                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при регистрации пользователя {Email}", request.Email);
                return StatusCode(500, new { message = "Произошла ошибка при регистрации" });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Некорректные данные", errors = ModelState });

                var user = await _usersService.AuthenticateAsync(request.Email, request.Password);
                if (user == null || !user.isAppruved)
                    return Unauthorized(new { message = "Неверные учетные данные или аккаунт не активирован" });

                var token = GenerateJwtToken(user);
                SetAuthCookie(token);

                _logger.LogInformation("Пользователь вошел в систему: {Email}", user.Email);
                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при входе пользователя {Email}", request.Email);
                return StatusCode(500, new { message = "Произошла ошибка при входе" });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            try
            {
                Response.Cookies.Delete(Program.AuthCookieName, new CookieOptions
                {
                    Path = "/",
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                });

                _logger.LogInformation("Пользователь вышел из системы");
                return Ok(new { message = "Вы успешно вышли из системы" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при выходе пользователя из системы");
                return StatusCode(500, new { message = "Произошла ошибка при выходе" });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserInfo>> GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var id))
                    return Unauthorized(new { message = "Неверный токен авторизации" });

                var user = await _usersService.GetUserByIdAsync(id);
                if (user == null)
                    return Unauthorized(new { message = "Пользователь не найден" });

                var userInfo = new UserInfo
                {
                    Id = user.id,
                    Email = user.Email,
                    Role = user.Role,
                    UsersName = user.UsersName
                };

                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении данных текущего пользователя");
                return StatusCode(500, new { message = "Произошла ошибка при получении данных пользователя" });
            }
        }

        private string GenerateJwtToken(Users user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UsersName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("id", user.id.ToString())
            };

           // var tokenExpiryMinutes = _config.GetValue<int>("Jwt:ExpiresMinutes", 120);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMonths(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private void SetAuthCookie(string token)
        {
            var tokenExpiryMinutes = _config.GetValue<int>("Jwt:ExpiresMinutes", 120);
            Response.Cookies.Append(
                Program.AuthCookieName,
                token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(tokenExpiryMinutes),
                    Path = "/"
                });
        }
    }

    // Упрощенные DTO классы
    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string UsersName { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}