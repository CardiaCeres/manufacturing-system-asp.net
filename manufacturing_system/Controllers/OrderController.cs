using ManufacturingSystem.Models;
using ManufacturingSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ManufacturingSystem.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public OrderController(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }

        // 取得訂單列表
        [HttpGet("my")]
        public async Task<IActionResult> GetMyOrders()
        {
            var currentUser = (User?)HttpContext.Items["User"];
            if (currentUser == null) return Unauthorized("無效使用者");

            if (currentUser.Role == UserRole.Manager)
            {
                // 管理者取得同部門訂單
                var orders = await _orderService.GetOrdersByDepartmentAsync(currentUser.Department);
                return Ok(orders);
            }
            else
            {
                // 普通使用者取得自己訂單
                var orders = await _orderService.GetOrdersByUserIdAsync(currentUser.Id);
                return Ok(orders);
            }
        }

        // 建立訂單
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            var currentUser = (User?)HttpContext.Items["User"];
            if (currentUser == null) return Unauthorized("無效使用者");

            if (string.IsNullOrEmpty(order.ProductName))
                return BadRequest("產品名稱為必填");

            if (currentUser.Role == UserRole.Manager)
            {
                // 管理者必須指定 UserId，並且指定的使用者要在同部門
                var targetUser = await _userService.GetUserByIdAsync(order.UserId);
                if (targetUser == null || targetUser.Department != currentUser.Department)
                    return BadRequest("指定使用者不存在或不在同部門");
            }
            else
            {
                // 普通使用者只能建立自己的訂單
                order.UserId = currentUser.Id;
            }

            var created = await _orderService.CreateOrderAsync(order);
            return Ok(created);
        }

        // 更新訂單
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateOrder(long id, [FromBody] Order order)
        {
            var currentUser = (User?)HttpContext.Items["User"];
            if (currentUser == null) return Unauthorized("無效使用者");

            var existing = await _orderService.GetOrderByIdAsync(id);
            if (existing == null) return NotFound("訂單不存在");

            // 權限檢查
            if (currentUser.Role == UserRole.Manager)
            {
                if (existing.User == null || existing.User.Department != currentUser.Department)
                    return Forbid("沒有權限更新此訂單");
            }
            else
            {
                if (existing.UserId != currentUser.Id)
                    return Forbid("沒有權限更新此訂單");
            }

            // 防止亂改 UserId
            order.Id = id;
            order.UserId = existing.UserId;

            var updated = await _orderService.UpdateOrderAsync(id, order);
            return Ok(updated);
        }

        // 刪除訂單
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteOrder(long id)
        {
            var currentUser = (User?)HttpContext.Items["User"];
            if (currentUser == null) return Unauthorized("無效使用者");

            var existing = await _orderService.GetOrderByIdAsync(id);
            if (existing == null) return NotFound("訂單不存在");

            // 權限檢查
            if (currentUser.Role == UserRole.Manager)
            {
                if (existing.User == null || existing.User.Department != currentUser.Department)
                    return Forbid("沒有權限刪除此訂單");
            }
            else
            {
                if (existing.UserId != currentUser.Id)
                    return Forbid("沒有權限刪除此訂單");
            }

            await _orderService.DeleteOrderAsync(id);
            return NoContent();
        }
    }
}
