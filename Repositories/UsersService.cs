using ShopPrimeVueServerAPI.Crud;
using ShopPrimeVueServerAPI.Models;

namespace ShopPrimeVueServerAPI.Repositories
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly ILogger<UsersService> _logger;

        public UsersService(IUsersRepository usersRepository, ILogger<UsersService> logger)
        {
            _usersRepository = usersRepository;
            _logger = logger;
        }

        public Task<IEnumerable<Users>> GetAllUsersAsync() => _usersRepository.GetAllAsync();

        public Task<Users?> GetUserByIdAsync(int id) => _usersRepository.GetByIdAsync(id);

        public Task<Users?> GetUserByEmailAsync(string email) => _usersRepository.GetByEmailAsync(email);

        public async Task<Users> RegisterUserAsync(Users user)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email обязателен");

            if (string.IsNullOrWhiteSpace(user.Password))
                throw new ArgumentException("Пароль обязателен");

              return await _usersRepository.CreateAsync(user);
        }

        public Task UpdateUserAsync(Users user) => _usersRepository.UpdateAsync(user);

        public Task DeleteUserAsync(int id) => _usersRepository.DeleteAsync(id);

        public Task<Users?> AuthenticateAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return Task.FromResult<Users?>(null);

            return _usersRepository.AuthenticateAsync(email, password);
        }
    }
}
