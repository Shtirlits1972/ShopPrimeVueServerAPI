using Dapper;
using Microsoft.Data.SqlClient;
using ShopPrimeVueServerAPI.Crud;
using ShopPrimeVueServerAPI.Models;
using System.Data;

namespace ShopPrimeVueServerAPI.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<UsersRepository> _logger;

        public UsersRepository(IConfiguration configuration, ILogger<UsersRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("SqlConnString")
                              ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Users>> GetAllAsync()
        {
            using var db = CreateConnection();
            try
            {
                const string query = @"SELECT Id, Email, Password, Role, UsersName, isAppruved 
                                     FROM Users";
                return await db.QueryAsync<Users>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении всех пользователей");
                throw;
            }
        }

        public async Task<Users?> GetByIdAsync(int id)
        {
            using var db = CreateConnection();
            try
            {
                const string query = @"SELECT Id, Email, Password, Role, UsersName, isAppruved 
                                     FROM Users WHERE Id = @Id";
                return await db.QueryFirstOrDefaultAsync<Users>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении пользователя по ID: {UserId}", id);
                throw;
            }
        }

        public async Task<Users?> GetByEmailAsync(string email)
        {
            using var db = CreateConnection();
            try
            {
                const string query = @"SELECT Id, Email, Password, Role, UsersName, isAppruved 
                                     FROM Users WHERE Email = @Email";
                return await db.QueryFirstOrDefaultAsync<Users>(query, new { Email = email });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении пользователя по email: {Email}", email);
                throw;
            }
        }

        public async Task<Users> CreateAsync(Users user)
        {
            using var db = CreateConnection();
            try
            {
                const string query = @"INSERT INTO Users (Email, Password, Role, UsersName, isAppruved) 
                                     OUTPUT INSERTED.* 
                                     VALUES (@Email, @Password, @Role, @UsersName, @isAppruved)";

                var createdUser = await db.QuerySingleAsync<Users>(query, new
                {
                    user.Email,
                    user.Password,
                    Role = user.Role ?? "user",
                    user.UsersName,
                    user.isAppruved
                });

                _logger.LogInformation("Создан пользователь: {Email} (ID: {UserId})", user.Email, createdUser.id);
                return createdUser;
            }
            catch (SqlException ex) when (ex.Number == 2627) // Unique constraint violation
            {
                _logger.LogWarning("Попытка создания пользователя с существующим email: {Email}", user.Email);
                throw new InvalidOperationException($"Пользователь с email '{user.Email}' уже существует", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании пользователя: {Email}", user.Email);
                throw;
            }
        }

        public async Task UpdateAsync(Users user)
        {
            using var db = CreateConnection();
            try
            {
                const string query = @"UPDATE Users 
                                     SET Email = @Email, Password = @Password, Role = @Role, 
                                         UsersName = @UsersName, isAppruved = @isAppruved 
                                     WHERE Id = @Id";

                var affectedRows = await db.ExecuteAsync(query, new
                {
                    user.Email,
                    user.Password,
                    user.Role,
                    user.UsersName,
                    user.isAppruved,
                    user.id
                });

                if (affectedRows == 0)
                    throw new KeyNotFoundException($"Пользователь с ID {user.id} не найден");

                _logger.LogInformation("Обновлен пользователь: {Email} (ID: {UserId})", user.Email, user.id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении пользователя: {Email} (ID: {UserId})",
                    user.Email, user.id);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            using var db = CreateConnection();
            try
            {
                const string query = "DELETE FROM Users WHERE Id = @Id";
                var affectedRows = await db.ExecuteAsync(query, new { Id = id });

                if (affectedRows == 0)
                    throw new KeyNotFoundException($"Пользователь с ID {id} не найден");

                _logger.LogInformation("Удален пользователь ID: {UserId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении пользователя ID: {UserId}", id);
                throw;
            }
        }

        public async Task<Users?> AuthenticateAsync(string email, string password)
        {
            using var db = CreateConnection();
            try
            {
                const string query = @"SELECT Id, Email, Password, Role, UsersName, isAppruved 
                                     FROM Users 
                                     WHERE Email = @Email AND Password = @Password AND isAppruved = 1";

                var user = await db.QueryFirstOrDefaultAsync<Users>(query, new { Email = email, Password = password });

                if (user != null)
                    _logger.LogInformation("Успешная авторизация: {Email}", email);
                else
                    _logger.LogWarning("Неудачная попытка авторизации: {Email}", email);

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при авторизации пользователя: {Email}", email);
                throw;
            }
        }
    }
}
