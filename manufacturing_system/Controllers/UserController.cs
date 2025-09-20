using ManufacturingSystem.Models;
using ManufacturingSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ManufacturingSystem.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService) => _userService = userService;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var validUser = await _userService.ValidateUserAsync(user.Username, user.Password);
            if (validUser == null) return Unauthorized("憑證錯誤，請確認帳號密碼");

            var token = Security.JwtHelper.GenerateToken(validUser.Username);
            return Ok(new { Token = token, User = validUser });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            var existing = await _userService.GetByUsernameAsync(user.Username);
            if (existing != null) return BadRequest("使用者名稱已存在");

            var newUser = await _userService.RegisterUserAsync(user);
            return Ok(newUser);
        }
    }
}
