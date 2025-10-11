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
                // 管理者取得本部門訂單
                var orders = await _orderService.GetOrdersByDepartmentAsync(currentUser.Department);
                return Ok(orders);
            }
            else
            {
                // 一般使用者取得自己訂單
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

            if (currentUser.Role != UserRole.Manager)
            {
            // 設定訂單使用者與部門
            order.UserId = currentUser.Id;
            order.Department = currentUser.Department;
            }

            try
            {
                var created = await _orderService.CreateOrderAsync(order);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"建立失敗: {ex.Message}");
            }
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
                if (existing.Department != currentUser.Department)
                    return Forbid("沒有權限更新其他部門的訂單");
            }
            else
            {
                if (existing.UserId != currentUser.Id)
                    return Forbid("沒有權限更新他人訂單");
            }

            // 保留原訂單 UserId 與 Department
            order.Id = id;
            order.UserId = existing.UserId;
            order.Department = existing.Department;

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

            // 權限檢查
            if (currentUser.Role == UserRole.Manager)
            {
                if (existing.Department != currentUser.Department)
                    return Forbid("沒有權限刪除此訂單");
            }
            else
            {
                if (existing.UserId != currentUser.Id)
                    return Forbid("沒有權限刪除此訂單");
            }

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
