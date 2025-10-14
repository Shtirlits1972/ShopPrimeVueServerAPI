using ShopPrimeVueServerAPI.Models;

namespace ShopPrimeVueServerAPI.Crud
{
    public interface IUsersRepository
    {
        Task<IEnumerable<Users>> GetAllAsync();
        Task<Users?> GetByIdAsync(int id);
        Task<Users?> GetByEmailAsync(string email);
        Task<Users> CreateAsync(Users user);
        Task UpdateAsync(Users user);
        Task DeleteAsync(int id);
        Task<Users?> AuthenticateAsync(string email, string password);
    }
}
