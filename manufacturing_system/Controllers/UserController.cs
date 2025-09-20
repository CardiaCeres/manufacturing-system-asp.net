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
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Username and Password are required");

            var validUser = await _userService.ValidateUserAsync(request.Username, request.Password);
            if (validUser == null)
                return Unauthorized("憑證錯誤，請確認帳號密碼");

            var token = Security.JwtHelper.GenerateToken(validUser.Username);
            return Ok(new 
            { 
                Token = token, 
                User = new 
                { 
                    validUser.Id, 
                    validUser.Username, 
                    validUser.Email 
                } 
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
                return BadRequest("Username and Password are required");

            var existing = await _userService.GetByUsernameAsync(user.Username);
            if (existing != null) return BadRequest("使用者名稱已存在");

            var newUser = await _userService.RegisterUserAsync(user);
            return Ok(new 
            { 
                newUser.Id, 
                newUser.Username, 
                newUser.Email 
            });
        }
    }
}
