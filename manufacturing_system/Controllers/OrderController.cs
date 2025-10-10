using ManufacturingSystem.Models;
using ManufacturingSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System;
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
                // 管理者可以看到所有訂單
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            else
            {
                // 普通使用者只能看到自己的訂單
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

            // 管理者可以指定 UserId
            if (currentUser.Role != UserRole.Manager)
            {
                order.UserId = currentUser.Id; // 普通使用者只能建立自己的訂單
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

            // 權限檢查：普通使用者只能更新自己的訂單
            if (currentUser.Role != UserRole.Manager && existing.UserId != currentUser.Id)
                return Forbid("沒有權限更新此訂單");

            try
            {
                var updated = await _orderService.UpdateOrderAsync(id, order);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"更新失敗: {ex.Message}");
            }
        }

        // 刪除訂單
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteOrder(long id)
        {
            var currentUser = (User?)HttpContext.Items["User"];
            if (currentUser == null) return Unauthorized("無效使用者");

            var existing = await _orderService.GetOrderByIdAsync(id);
            if (existing == null) return NotFound("訂單不存在");

            // 權限檢查：普通使用者只能刪自己的訂單
            if (currentUser.Role != UserRole.Manager && existing.UserId != currentUser.Id)
                return Forbid("沒有權限刪除此訂單");

            try
            {
                await _orderService.DeleteOrderAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"刪除失敗: {ex.Message}");
            }
        }
    }
}
