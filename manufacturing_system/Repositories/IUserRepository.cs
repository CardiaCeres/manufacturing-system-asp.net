using ManufacturingSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManufacturingSystem.Repositories
{
    public interface IUserRepository
    {
        // 查詢
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByResetTokenAsync(string resetToken);
        Task<User?> GetByIdAsync(long id); // 用 Id 查使用者

        // CRUD
        Task<User> AddOrUpdateAsync(User user);
        Task DeleteAsync(long id);

        // 管理者功能
        Task<List<User>> GetByDepartmentAsync(string department); // 取得同部門所有使用者
        Task<List<User>> GetAllAsync(); // 取得全部使用者
    }
}
