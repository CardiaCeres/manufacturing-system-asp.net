using System;

namespace ManufacturingSystem.Models
{
    // 定義使用者角色
    public enum UserRole
    {
        User,       // 一般使用者
        Manager     // 管理者
    }

    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // 部門欄位
        public string Department { get; set; } = string.Empty;

        // 使用者角色
        public UserRole Role { get; set; } = UserRole.User;

        // 密碼重置相關
        public string? ResetToken { get; set; }
        public DateTime? TokenExpiry { get; set; }
    }
}
