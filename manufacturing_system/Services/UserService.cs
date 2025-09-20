using ManufacturingSystem.Models;
using ManufacturingSystem.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
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

        // 註冊
        public async Task<User> RegisterUserAsync(User user)
        {
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
            return Task.FromResult(user.ResetToken == token && user.TokenExpiry.HasValue && user.TokenExpiry > DateTime.UtcNow);
        }
    }
}
