using ManufacturingSystem.Models;
using ManufacturingSystem.Services;
using ManufacturingSystem.Security;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ManufacturingSystem.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public UserController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        // 登入
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var validUser = await _userService.ValidateUserAsync(user.Username, user.Password);
            if (validUser == null) 
                return Unauthorized("帳號或密碼錯誤");

            // JWT 包含 Username, Role, Department
            var token = JwtHelper.GenerateToken(
                validUser.Username, 
                validUser.Role.ToString(), 
                validUser.Department
            );

            return Ok(new { Token = token, User = validUser });
        }

        // ✅ 註冊
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
                return BadRequest("帳號與密碼為必填");

            var existingUser = await _userService.GetByUsernameAsync(user.Username);
            if (existingUser != null)
                return BadRequest("使用者名稱已存在");

            // 設定預設角色
            if (user.Role == 0) 
                user.Role = UserRole.User;

            // 預設部門
            user.Department ??= "一般部門";

            var newUser = await _userService.RegisterUserAsync(user);
            return Ok(newUser);
        }

        // 取得使用者資料（自己）
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var currentUser = (User?)HttpContext.Items["User"];
            if (currentUser == null) return Unauthorized();

            var user = await _userService.GetUserByIdAsync(currentUser.Id);
            return Ok(user);
        }

        // ✅ 忘記密碼（修正查詢方式）
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
                return NotFound("找不到此 Email 對應的使用者");

            var token = await _userService.GenerateResetTokenAsync(user);
            var resetUrl = $"https://yourapp.com/reset-password?token={token}&username={user.Username}";
            await _emailService.SendResetPasswordEmailAsync(user.Email, resetUrl);

            return Ok("重設密碼信已寄出");
        }

        // 重設密碼
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var user = await _userService.GetByUsernameAsync(request.Username);
            if (user == null) return NotFound("找不到此使用者");

            var isValid = await _userService.IsResetTokenValidAsync(user, request.Token);
            if (!isValid) return BadRequest("無效或過期的重設 Token");

            await _userService.ResetPasswordAsync(user, request.NewPassword);
            return Ok("密碼重設成功");
        }

        // 管理者：取得部門內使用者
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var currentUser = (User?)HttpContext.Items["User"];
            if (currentUser == null || currentUser.Role != UserRole.Manager) return Forbid();

            var users = await _userService.GetUsersByDepartmentAsync(currentUser.Department);
            return Ok(users);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] User user)
        {
            var currentUser = (User?)HttpContext.Items["User"];
            if (currentUser == null || currentUser.Role != UserRole.Manager) return Forbid();

            user.Id = id;
            var updated = await _userService.UpdateUserAsync(user);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var currentUser = (User?)HttpContext.Items["User"];
            if (currentUser == null || currentUser.Role != UserRole.Manager) return Forbid();

            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
    }

    // DTO
    public class ResetPasswordRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
