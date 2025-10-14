using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopPrimeVueServerAPI.Models;
using ShopPrimeVueServerAPI.Repositories;

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ShopPrimeVueServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUsersService usersService, ILogger<UsersController> logger)
        {
            _usersService = usersService;
            _logger = logger;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(Roles = "admin")] // Пример ограничения доступа
        public async Task<ActionResult<IEnumerable<Users>>> Get()
        {
            try
            {
                var users = await _usersService.GetAllUsersAsync();

                _logger.LogInformation("Получен список всех пользователей. Количество: {Count}", users.Count());
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка пользователей");
                return StatusCode(500, new { message = "Произошла ошибка при получении пользователей" });
            }
        }

        // GET api/Users/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserInfo>> Get(int id)
        {
            try
            {
                var user = await _usersService.GetUserByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Пользователь с ID {UserId} не найден", id);
                    return NotFound(new { message = "Пользователь не найден" });
                }

                // Проверяем, что пользователь запрашивает свои данные или имеет права администратора
                var currentUserId = User.FindFirst("id")?.Value;
                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (currentUserId != id.ToString() && currentUserRole != "admin")
                {
                    _logger.LogWarning("Попытка несанкционированного доступа к данным пользователя {RequestedUserId} пользователем {CurrentUserId}",
                        id, currentUserId);
                    return Forbid();
                }

                _logger.LogInformation("Получены данные пользователя ID: {UserId}", id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении пользователя ID: {UserId}", id);
                return StatusCode(500, new { message = "Произошла ошибка при получении пользователя" });
            }
        }

        // POST api/Users
        [HttpPost]
        [Authorize(Roles = "admin")] // Пример ограничения доступа
        public async Task<ActionResult<UserInfo>> Post([FromBody] CreateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Некорректные данные при создании пользователя: {Errors}",
                        string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                    return BadRequest(new { message = "Некорректные данные", errors = ModelState });
                }

                var user = new Users
                {
                    Email = request.Email,
                    Password = request.Password, // В будущем будет хеширование
                    UsersName = request.UsersName,
                    Role = request.Role ?? "user",
                    isAppruved = request.isAppruved
                };

                var createdUser = await _usersService.RegisterUserAsync(user);

                var userInfo = new UserInfo
                {
                    Id = createdUser.id,
                    Email = createdUser.Email,
                    Role = createdUser.Role,
                    UsersName = createdUser.UsersName
                };

                _logger.LogInformation("Создан новый пользователь: {Email} (ID: {UserId})", createdUser.Email, createdUser.id);
                return CreatedAtAction(nameof(Get), new { id = createdUser.id }, userInfo);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("уже существует"))
            {
                _logger.LogWarning("Попытка создания пользователя с существующим email: {Email}", request.Email);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании пользователя: {Email}", request.Email);
                return StatusCode(500, new { message = "Произошла ошибка при создании пользователя" });
            }
        }

        // PUT api/Users/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> Put(int id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Некорректные данные при обновлении пользователя ID: {UserId}", id);
                    return BadRequest(new { message = "Некорректные данные", errors = ModelState });
                }

                // Получаем текущего пользователя для проверки прав
                var currentUserId = User.FindFirst("id")?.Value;
                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

                // Проверяем, что пользователь обновляет свои данные или имеет права администратора
                if (currentUserId != id.ToString() && currentUserRole != "admin")
                {
                    _logger.LogWarning("Попытка несанкционированного обновления пользователя {RequestedUserId} пользователем {CurrentUserId}",
                        id, currentUserId);
                    return Forbid();
                }

                var existingUser = await _usersService.GetUserByIdAsync(id);
                if (existingUser == null)
                {
                    _logger.LogWarning("Пользователь с ID {UserId} не найден при обновлении", id);
                    return NotFound(new { message = "Пользователь не найден" });
                }

                // Обновляем только разрешенные поля
                existingUser.UsersName = request.UsersName;
                existingUser.Email = request.Email;

                // Только админы могут менять роль и статус активации
                if (currentUserRole == "admin")
                {
                    existingUser.Role = request.Role;
                    existingUser.isAppruved = request.isAppruved;
                }

                // Если указан новый пароль
                if (!string.IsNullOrEmpty(request.NewPassword))
                {
                    existingUser.Password = request.NewPassword; // В будущем будет хеширование
                }

                await _usersService.UpdateUserAsync(existingUser);

                _logger.LogInformation("Обновлен пользователь ID: {UserId}", id);
                return Ok(new { message = "Пользователь успешно обновлен" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении пользователя ID: {UserId}", id);
                return StatusCode(500, new { message = "Произошла ошибка при обновлении пользователя" });
            }
        }

        // DELETE api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")] // Пример ограничения доступа
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var existingUser = await _usersService.GetUserByIdAsync(id);
                if (existingUser == null)
                {
                    _logger.LogWarning("Пользователь с ID {UserId} не найден при удалении", id);
                    return NotFound(new { message = "Пользователь не найден" });
                }

                await _usersService.DeleteUserAsync(id);

                _logger.LogInformation("Удален пользователь ID: {UserId}", id);
                return Ok(new { message = "Пользователь успешно удален" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении пользователя ID: {UserId}", id);
                return StatusCode(500, new { message = "Произошла ошибка при удалении пользователя" });
            }
        }
    }

    // DTO классы для запросов
    public class CreateUserRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(250)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string UsersName { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Role { get; set; }

        public bool isAppruved { get; set; } = true;
    }

    public class UpdateUserRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(250)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string UsersName { get; set; } = string.Empty;

        [StringLength(250)]
        public string Role { get; set; } = "user";

        public bool isAppruved { get; set; } = true;

        [StringLength(250, MinimumLength = 6)]
        public string? NewPassword { get; set; }
    }
}