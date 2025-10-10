using ManufacturingSystem.Services;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace ManufacturingSystem.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context, IUserService userService)
        {
            try
            {
                // 從 Authorization Header 取得 Bearer token
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                if (!string.IsNullOrEmpty(token))
                {
                    var principal = Security.JwtHelper.ValidateToken(token);
                    if (principal != null)
                    {
                        // 取得 Username、Role、Department
                        var username = principal.Identity?.Name;
                        var role = principal.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                        var department = principal.FindFirst("Department")?.Value;

                        if (!string.IsNullOrEmpty(username))
                        {
                            var user = await userService.GetByUsernameAsync(username);
                            if (user != null)
                            {
                                // 更新 User 資料（角色、部門）以便 Controller 使用
                                user.Role = role != null ? Enum.Parse<Models.UserRole>(role) : user.Role;
                                user.Department = department ?? user.Department;

                                context.Items["User"] = user;
                            }
                        }
                    }
                }
            }
            catch
            {
                // 若 Token 無效，不阻止請求，Controller 再判斷
            }

            await _next(context);
        }
    }
}
