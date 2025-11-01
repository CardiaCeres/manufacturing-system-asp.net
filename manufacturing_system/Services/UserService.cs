using ManufacturingSystem.Models;
using ManufacturingSystem.Repositories;
using System;
using System.Threading.Tasks;
using BCrypt.Net;

namespace ManufacturingSystem.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly int BcryptWorkFactor = 12;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // 驗證使用者帳密
        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null) return null;

            bool valid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            return valid ? user : null;
        }

        // 註冊
        public async Task<User> RegisterUserAsync(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, BcryptWorkFactor);
            return await _userRepository.AddOrUpdateAsync(user);
        }

        // 根據 Email 找使用者
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        // 根據 Username 找使用者
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        // 產生重設 Token
        public async Task<string> GenerateResetTokenAsync(User user)
        {
            var token = Guid.NewGuid().ToString();
            user.ResetToken = token;

            // UTC
            user.TokenExpiry = DateTime.UtcNow.AddHours(1);

            await _userRepository.AddOrUpdateAsync(user);
            return token;
        }

        // 驗證 Reset Token 是否有效
        public async Task<bool> IsResetTokenValidAsync(User user, string token)
        {
            if (user.ResetToken != token) return false;
            if (user.TokenExpiry == null) return false;

            return user.TokenExpiry.Value > DateTime.UtcNow;
        }

        // 重設密碼
        public async Task ResetPasswordAsync(User user, string newPassword)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword, BcryptWorkFactor);
            user.ResetToken = null;

            // 確保 UTC
            if (user.TokenExpiry != null)
                user.TokenExpiry = DateTime.SpecifyKind(user.TokenExpiry.Value, DateTimeKind.Utc);

            await _userRepository.AddOrUpdateAsync(user);
        }

        // 管理者取得同部門使用者
        public async Task<User[]> GetUsersByDepartmentAsync(string department)
        {
            return await _userRepository.GetUsersByDepartmentAsync(department);
        }

        // 更新使用者
        public async Task<User> UpdateUserAsync(User user)
        {
            return await _userRepository.AddOrUpdateAsync(user);
        }

        // 刪除使用者
        public async Task DeleteUserAsync(long id)
        {
            await _userRepository.DeleteAsync(id);
        }

        // 根據 Reset Token 找使用者
        public async Task<User?> GetByResetTokenAsync(string token)
        {
            return await _userRepository.GetByResetTokenAsync(token);
        }
    }
}
