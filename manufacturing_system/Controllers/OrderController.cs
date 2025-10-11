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
                var orders = await _orderService.GetOrdersByDepartmentAsync(currentUser.Department);
                return Ok(orders);
            }
            else
            {
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
                if (order.UserId == 0)
                    return BadRequest("請指定要建立訂單的使用者 ID");

                var targetUser = await _userService.GetUserByIdAsync(order.UserId);
                if (targetUser == null || targetUser.Department != currentUser.Department)
                    return BadRequest("指定使用者不存在或不在同部門");

                order.Department = currentUser.Department;
            }
            else
            {
                order.UserId = currentUser.Id;
                order.Department = currentUser.Department;
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

            if (currentUser.Role == UserRole.Manager)
            {
                if (existing.User == null || existing.User.Department != currentUser.Department)
                    return Forbid("沒有權限更新其他部門的訂單");
            }
            else
            {
                if (existing.UserId != currentUser.Id)
                    return Forbid("沒有權限更新他人訂單");
            }

            order.Id = id;
            order.UserId = existing.UserId;
            order.Department = existing.Department;

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
