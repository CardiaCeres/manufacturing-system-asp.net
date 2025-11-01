using ManufacturingSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManufacturingSystem.Services
{
    public interface IUserService
    {
        // 驗證使用者帳號密碼
        Task<User?> ValidateUserAsync(string username, string password);

        // 根據使用者名稱取得 User
        Task<User?> GetByUsernameAsync(string username);

        // 根據 Email 取得使用者
        Task<User?> GetByEmailAsync(string email);

        // 根據 Token 取得使用者（新增，用於重設密碼）
        Task<User?> GetByResetTokenAsync(string token);

        // 註冊新使用者（可指定部門與角色）
        Task<User> RegisterUserAsync(User user);

        // 產生密碼重設 Token
        Task<string> GenerateResetTokenAsync(User user);

        // 重設密碼
        Task ResetPasswordAsync(User user, string newPassword);

        // 驗證密碼重設 Token 是否有效
        Task<bool> IsResetTokenValidAsync(User user, string token);

        // 根據使用者 Id 取得使用者
        Task<User?> GetUserByIdAsync(long userId);

        // 取得同部門所有使用者
        Task<IEnumerable<User>> GetUsersByDepartmentAsync(string department);

        // 更新使用者資料（包含角色、部門）
        Task<User> UpdateUserAsync(User user);

        // 刪除使用者
        Task DeleteUserAsync(long userId);
    }
}
