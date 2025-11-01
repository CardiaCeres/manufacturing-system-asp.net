using ManufacturingSystem.Models;
using ManufacturingSystem.Services;
using ManufacturingSystem.Security;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ManufacturingSystem.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public UserController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        // ✅ 登入
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var validUser = await _userService.ValidateUserAsync(request.Username, request.Password);
            if (validUser == null)
                return Unauthorized("憑證錯誤，請確認帳號密碼");

            var token = JwtHelper.GenerateToken(validUser.Username, validUser.Role.ToString(), validUser.Department);
            return Ok(new { Token = token, User = validUser });
        }

        // ✅ 註冊
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var existing = await _userService.GetByUsernameAsync(request.Username);
            if (existing != null)
                return BadRequest("使用者名稱已存在");

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                Department = request.Department ?? string.Empty,
                Role = UserRole.User
            };

            var newUser = await _userService.RegisterUserAsync(user);
            return Ok(newUser);
        }

        // ✅ 取得登入者資訊
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var currentUser = (User?)HttpContext.Items["User"];
            if (currentUser == null) return Unauthorized();

            var user = await _userService.GetUserByIdAsync(currentUser.Id);
            return Ok(user);
        }

        // ✅ 忘記密碼：寄出重設信件
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _userService.GetByEmailAsync(request.Email);
            if (user == null)
                return NotFound("找不到此使用者");

            var token = await _userService.GenerateResetTokenAsync(user);

            var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL")
                              ?? throw new InvalidOperationException("FRONTEND_URL 未設定");

            var resetUrl = $"{frontendUrl.TrimEnd('/')}/reset-password?token={token}";

            try
            {
                await _emailService.SendResetPasswordEmailAsync(user.Email, resetUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"寄送重設密碼信失敗: {ex.Message}");
                return StatusCode(500, "寄送重設密碼信失敗，請稍後再試");
            }

            return Ok("重設密碼信已寄出");
        }

        // ✅ 重設密碼：只用 Token 查找 User（不需 Username）
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var user = await _userService.GetByResetTokenAsync(request.Token);
            if (user == null)
                return NotFound("無效或過期的重設 Token");

            var isValid = await _userService.IsResetTokenValidAsync(user, request.Token);
            if (!isValid)
                return BadRequest("無效或過期的重設 Token");

            await _userService.ResetPasswordAsync(user, request.NewPassword);

            return Ok("密碼重設成功");
        }

        // ✅ 管理者：查詢同部門使用者
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var currentUser = (User?)HttpContext.Items["User"];
            if (currentUser == null || currentUser.Role != UserRole.Manager)
                return Forbid();

            var users = await _userService.GetUsersByDepartmentAsync(currentUser.Department);
            return Ok(users);
        }

        // ✅ 管理者：更新使用者
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

        // ✅ 管理者：刪除使用者
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

    // ✅ DTOs
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Department { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
