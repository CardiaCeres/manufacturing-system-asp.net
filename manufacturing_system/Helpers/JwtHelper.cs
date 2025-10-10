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
        private static readonly string Secret = Environment.GetEnvironmentVariable("JWT_SECRET")
                                               ?? throw new InvalidOperationException("JWT_SECRET environment variable is not set.");
        private static readonly byte[] Key = Encoding.UTF8.GetBytes(Secret);

        /// <summary>
        /// 生成 JWT Token
        /// </summary>
        /// <param name="username">使用者名稱</param>
        /// <param name="role">使用者角色</param>
        /// <param name="department">使用者部門</param>
        /// <param name="expireMinutes">過期時間 (分鐘)，預設 1 天</param>
        /// <returns>JWT 字串</returns>
        public static string GenerateToken(string username, string role, string department, int expireMinutes = 1440)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // 建立 Claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim("Department", department)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// 驗證 JWT Token，並回傳 ClaimsPrincipal
        /// </summary>
        /// <param name="token">JWT 字串</param>
        /// <returns>ClaimsPrincipal 或 null</returns>
        public static ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Key),
                    ClockSkew = TimeSpan.Zero // 可防止小時差造成驗證失敗
                }, out var validatedToken);

                return principal;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 驗證 Token 並取得 Username
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Username 或 null</returns>
        public static string? GetUsernameFromToken(string token)
        {
            var principal = ValidateToken(token);
            return principal?.Identity?.Name;
        }

        /// <summary>
        /// 驗證 Token 並取得 Role
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Role 或 null</returns>
        public static string? GetRoleFromToken(string token)
        {
            var principal = ValidateToken(token);
            return principal?.FindFirst(ClaimTypes.Role)?.Value;
        }

        /// <summary>
        /// 驗證 Token 並取得 Department
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Department 或 null</returns>
        public static string? GetDepartmentFromToken(string token)
        {
            var principal = ValidateToken(token);
            return principal?.FindFirst("Department")?.Value;
        }
    }
}
