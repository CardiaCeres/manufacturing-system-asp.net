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

        // 🔹 登入
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var validUser = await _userService.ValidateUserAsync(user.Username, user.Password);
            if (validUser == null) return Unauthorized("帳號或密碼錯誤");

            var token = JwtHelper.GenerateToken(
                validUser.Username,
                validUser.Role.ToString(),
                validUser.Department
            );

            return Ok(new
            {
                Token = token,
                User = new
                {
                    validUser.Id,
                    validUser.Username,
                    validUser.Email,
                    validUser.Role,
                    validUser.Department
                }
            });
        }

        // 🔹 註冊
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            var existing = await _userService.GetByUsernameAsync(user.Username);
            if (existing != null) return BadRequest("使用者名稱已存在");

            // 預設角色
            if (string.IsNullOrEmpty(user.Role.ToString()))
                user.Role = UserRole.User;

            var newUser = await _userService.RegisterUserAsync(user);
            return Ok(newUser);
        }

        // 🔹 取得目前使用者資料
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var currentUser = (User?)HttpContext.Items["User"];
            if (currentUser == null) return Unauthorized();

            var user = await _userService.GetUserByIdAsync(currentUser.Id);
            return Ok(user);
        }

        // 🔹 忘記密碼：寄出重設連結
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _userService.GetByEmailAsync(request.Email);
            if (user == null) return NotFound("找不到此使用者");

            var token = await _userService.GenerateResetTokenAsync(user);
            var resetUrl = $"{request.BaseUrl}/reset-password?token={token}&username={user.Username}";

            await _emailService.SendResetPasswordEmailAsync(user.Email, resetUrl);
            return Ok("重設密碼信已寄出");
        }

        // 🔹 重設密碼
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var user = await _userService.GetByUsernameAsync(request.Username);
            if (user == null) return NotFound("找不到此使用者");

            var isValid = await _userService.IsResetTokenValidAsync(user, request.Token);
            if (!isValid) return BadRequest("無效或過期的重設 Token");

            await _userService.ResetPasswordAsync(user, request.NewPassword);
            return Ok("密碼已重設成功");
        }

        // 🔹 取得部門使用者（管理者）
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var currentUser = (User?)HttpContext.Items["User"];
            if (currentUser == null || currentUser.Role != UserRole.Manager)
                return Forbid();

            var users = await _userService.GetUsersByDepartmentAsync(currentUser.Department);
            return Ok(users);
        }

        // 🔹 更新使用者資料
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] User user)
        {
            var currentUser = (User?)HttpContext.Items["User"];
            if (currentUser == null || currentUser.Role != UserRole.Manager)
                return Forbid();

            user.Id = id;
            var updated = await _userService.UpdateUserAsync(user);
            return Ok(updated);
        }

        // 🔹 刪除使用者
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var currentUser = (User?)HttpContext.Items["User"];
            if (currentUser == null || currentUser.Role != UserRole.Manager)
                return Forbid();

            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
    }

    // ✅ 忘記密碼請求
    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://yourapp.com";
    }

    // ✅ 重設密碼請求
    public class ResetPasswordRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
