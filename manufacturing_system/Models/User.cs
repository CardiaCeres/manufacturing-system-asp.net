using System;

namespace ManufacturingSystem.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ResetToken { get; set; }
        public DateTime? TokenExpiry { get; set; }
    }
}
