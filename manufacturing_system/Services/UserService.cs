using ManufacturingSystem.Models;
using ManufacturingSystem.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BCrypt.Net;

namespace ManufacturingSystem.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // 登入驗證
        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || string.IsNullOrEmpty(user.Password))
                return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success ? user : null;
        }

        // 查找使用者
        public async Task<User?> GetByUsernameAsync(string username) =>
            await _userRepository.GetByUsernameAsync(username);

        public async Task<User?> GetByEmailAsync(string email) =>
            await _userRepository.GetByEmailAsync(email);

        public async Task<User?> GetUserByIdAsync(long userId) =>
            await _userRepository.GetByIdAsync(userId);

        // 註冊新使用者
        public async Task<User> RegisterUserAsync(User user)
        {
            if (user.Role == 0)
                user.Role = UserRole.User;

            user.Password = _passwordHasher.HashPassword(user, user.Password);
            return await _userRepository.AddOrUpdateAsync(user);
        }

        // 產生重設密碼 Token
        public async Task<string> GenerateResetTokenAsync(User user)
        {
            var token = Guid.NewGuid().ToString();
            user.ResetToken = token;
            user.TokenExpiry = DateTime.UtcNow.AddHours(1); // 一定使用 UTC
            await _userRepository.AddOrUpdateAsync(user);
            return token;
        }

        // 根據 Token 查找使用者（自動驗證過期）
        public async Task<User?> GetByResetTokenAsync(string token)
        {
            var user = await _userRepository.GetByResetTokenAsync(token);
            if (user == null) return null;

            // 自動清除過期 Token
            if (!user.TokenExpiry.HasValue || user.TokenExpiry <= DateTime.UtcNow)
            {
                user.ResetToken = null;
                await _userRepository.AddOrUpdateAsync(user);
                return null;
            }

            return user;
        }

        // 驗證 Token 是否有效
        public Task<bool> IsResetTokenValidAsync(User user, string token)
        {
            return Task.FromResult(
                user != null &&
                user.ResetToken == token &&
                user.TokenExpiry.HasValue &&
                user.TokenExpiry > DateTime.UtcNow
            );
        }

        // 重設密碼
        public async Task ResetPasswordAsync(User user, string newPassword)
{
    user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
    user.ResetToken = null;

    // 修正 TokenExpiry 為 UTC
    if (user.TokenExpiry != null)
        user.TokenExpiry = DateTime.SpecifyKind(user.TokenExpiry.Value, DateTimeKind.Utc);

    await _userRepository.AddOrUpdateAsync(user); // 寫入 DB
}


        // 取得同部門使用者
        public async Task<IEnumerable<User>> GetUsersByDepartmentAsync(string department)
        {
            return await _userRepository.GetByDepartmentAsync(department);
        }

        // 更新使用者資料
        public async Task<User> UpdateUserAsync(User user)
        {
            if (!string.IsNullOrEmpty(user.Password))
                user.Password = _passwordHasher.HashPassword(user, user.Password);

            return await _userRepository.AddOrUpdateAsync(user);
        }

        // 刪除使用者
        public async Task DeleteUserAsync(long userId)
        {
            await _userRepository.DeleteAsync(userId);
        }
    }
}
