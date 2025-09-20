using ManufacturingSystem.Models;
using System.Threading.Tasks;

namespace ManufacturingSystem.Services
{
    public interface IUserService
    {
        Task<User?> ValidateUserAsync(string username, string password);
        Task<User?> GetByUsernameAsync(string username);
        Task<User> RegisterUserAsync(User user);
        Task<string> GenerateResetTokenAsync(User user);
        Task ResetPasswordAsync(User user, string newPassword);
        Task<bool> IsResetTokenValidAsync(User user, string token);
    }
}
