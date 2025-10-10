using ManufacturingSystem.Models;
using ManufacturingSystem.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            if (user == null) return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success ? user : null;
        }

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _userRepository.GetByUsernameAsync(username);

        public async Task<User?> GetUserByIdAsync(long userId) =>
            await _userRepository.GetByIdAsync(userId);

        // 註冊
        public async Task<User> RegisterUserAsync(User user)
        {
            // 預設角色為 User
            if (user.Role == 0) user.Role = UserRole.User;

            // 加密密碼
            user.Password = _passwordHasher.HashPassword(user, user.Password);
            return await _userRepository.AddOrUpdateAsync(user);
        }

        // 產生重設密碼 Token
        public async Task<string> GenerateResetTokenAsync(User user)
        {
            var token = Guid.NewGuid().ToString();
            user.ResetToken = token;
            user.TokenExpiry = DateTime.UtcNow.AddHours(1);
            await _userRepository.AddOrUpdateAsync(user);
            return token;
        }

        // 重設密碼
        public async Task ResetPasswordAsync(User user, string newPassword)
        {
            user.Password = _passwordHasher.HashPassword(user, newPassword);
            user.ResetToken = null;
            user.TokenExpiry = null;
            await _userRepository.AddOrUpdateAsync(user);
        }

        // 驗證 Token
        public Task<bool> IsResetTokenValidAsync(User user, string token)
        {
            return Task.FromResult(user.ResetToken == token &&
                                   user.TokenExpiry.HasValue &&
                                   user.TokenExpiry > DateTime.UtcNow);
        }

        // 取得同部門使用者列表
        public async Task<IEnumerable<User>> GetUsersByDepartmentAsync(string department)
        {
            return await _userRepository.GetByDepartmentAsync(department);
        }

        // 更新使用者（管理者可修改角色與部門）
        public async Task<User> UpdateUserAsync(User user)
        {
            // 如果有修改密碼，也要加密
            if (!string.IsNullOrEmpty(user.Password))
            {
                user.Password = _passwordHasher.HashPassword(user, user.Password);
            }
            return await _userRepository.AddOrUpdateAsync(user);
        }

        // 刪除使用者
        public async Task DeleteUserAsync(long userId)
        {
            await _userRepository.DeleteAsync(userId);
        }
    }
}
