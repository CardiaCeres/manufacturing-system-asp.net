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
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                var username = Security.JwtHelper.ValidateToken(token);
                if (username != null)
                {
                    var user = await userService.GetByUsernameAsync(username);
                    if (user != null) context.Items["User"] = user;
                }
            }

            await _next(context);
        }
    }
}
