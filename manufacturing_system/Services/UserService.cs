using ManufacturingSystem.Models;
using ManufacturingSystem.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BCrypt.Net;

namespace ManufacturingSystem.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // 驗證使用者帳號密碼
        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || string.IsNullOrEmpty(user.Password))
                return null;

            return BCrypt.Net.BCrypt.Verify(password, user.Password) ? user : null;
        }

        // 根據使用者名稱取得 User
        public async Task<User?> GetByUsernameAsync(string username) =>
            await _userRepository.GetByUsernameAsync(username);

        // 根據 Email 取得使用者
        public async Task<User?> GetByEmailAsync(string email) =>
            await _userRepository.GetByEmailAsync(email);

        // 根據 Token 取得使用者（重設密碼用）
        public async Task<User?> GetByResetTokenAsync(string token)
        {
            var user = await _userRepository.GetByResetTokenAsync(token);
            if (user == null) return null;

            if (!user.TokenExpiry.HasValue || user.TokenExpiry.Value <= DateTime.UtcNow)
            {
                user.ResetToken = null;
                user.TokenExpiry = null;
                await _userRepository.AddOrUpdateAsync(user);
                return null;
            }

            return user;
        }

        // 註冊新使用者
        public async Task<User> RegisterUserAsync(User user)
        {
            if (user.Role == 0)
                user.Role = UserRole.User;

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            return await _userRepository.AddOrUpdateAsync(user);
        }

        // 產生密碼重設 Token
        public async Task<string> GenerateResetTokenAsync(User user)
        {
            var token = Guid.NewGuid().ToString();
            user.ResetToken = token;
            user.TokenExpiry = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(1), DateTimeKind.Utc);
            await _userRepository.AddOrUpdateAsync(user);
            return token;
        }

        // 重設密碼
        public async Task ResetPasswordAsync(User user, string newPassword)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.ResetToken = null;

            if (user.TokenExpiry != null)
                user.TokenExpiry = DateTime.SpecifyKind(user.TokenExpiry.Value, DateTimeKind.Utc);

            await _userRepository.AddOrUpdateAsync(user);
        }

        // 驗證密碼重設 Token 是否有效
        public Task<bool> IsResetTokenValidAsync(User user, string token)
        {
            return Task.FromResult(
                user != null &&
                user.ResetToken == token &&
                user.TokenExpiry.HasValue &&
                user.TokenExpiry.Value > DateTime.UtcNow
            );
        }

        // 根據使用者 Id 取得使用者
        public async Task<User?> GetUserByIdAsync(long userId) =>
            await _userRepository.GetByIdAsync(userId);

        // 取得同部門所有使用者
        public async Task<IEnumerable<User>> GetUsersByDepartmentAsync(string department) =>
            await _userRepository.GetByDepartmentAsync(department);

        // 更新使用者資料（包含角色、部門）
        public async Task<User> UpdateUserAsync(User user)
        {
            if (!string.IsNullOrEmpty(user.Password))
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            return await _userRepository.AddOrUpdateAsync(user);
        }

        // 刪除使用者
        public async Task DeleteUserAsync(long userId) =>
            await _userRepository.DeleteAsync(userId);
    }
}
