using ShopPrimeVueServerAPI.Models;

namespace ShopPrimeVueServerAPI.Repositories
{
    public interface IUsersService
    {
        Task<IEnumerable<Users>> GetAllUsersAsync();
        Task<Users?> GetUserByIdAsync(int id);
        Task<Users?> GetUserByEmailAsync(string email);
        Task<Users> RegisterUserAsync(Users user);
        Task UpdateUserAsync(Users user);
        Task DeleteUserAsync(int id);
        Task<Users?> AuthenticateAsync(string email, string password);
    }
}
