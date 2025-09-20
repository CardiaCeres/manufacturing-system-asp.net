using ManufacturingSystem.Models;
using System.Threading.Tasks;

namespace ManufacturingSystem.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByResetTokenAsync(string resetToken);
        Task<User> AddOrUpdateAsync(User user);
    }
}
