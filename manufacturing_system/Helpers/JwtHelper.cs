using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ManufacturingSystem.Security
{
    public static class JwtHelper
    {
        // 從環境變數讀取 JWT_SECRET，如果沒設定就拋例外
        private static readonly string Secret = Environment.GetEnvironmentVariable("JWT_SECRET") // 建議長度 >= 256 bits (32 bytes)
                                               ?? throw new InvalidOperationException("JWT_SECRET environment variable is not set.");
        private static readonly byte[] Key = Encoding.UTF8.GetBytes(Secret);

        public static string GenerateToken(string username, int expireMinutes = 1440)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Key),
                    ValidateIssuerSigningKey = true
                }, out var validatedToken);

                return principal.Identity?.Name;
            }
            catch
            {
                return null;
            }
        }
    }
}
