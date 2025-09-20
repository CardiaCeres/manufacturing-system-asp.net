using System;

namespace ManufacturingSystem.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? ResetToken { get; set; }
        public DateTime? TokenExpiry { get; set; }
    }
}
