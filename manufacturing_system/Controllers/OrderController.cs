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
            if (currentUser == null)
                return Unauthorized("無效使用者");

            var orders = currentUser.Role == UserRole.Manager
                ? await _orderService.GetOrdersByDepartmentAsync(currentUser.Department)
                : await _orderService.GetOrdersByUserIdAsync(currentUser.Id);

            return Ok(orders);
        }

        // 建立訂單
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            var currentUser = (User?)HttpContext.Items["User"];
            if (currentUser == null)
                return Unauthorized("無效使用者");

            // ✅ 共用驗證方法
            var validationError = ValidateOrder(order, true);
            if (validationError != null)
                return validationError;

            // ✅ 設定屬性（自動填入）
            if (currentUser.Role != UserRole.Manager)
            {
                order.UserId = currentUser.Id;
                order.Department = currentUser.Department;
            }
            else
            {
                order.Department ??= currentUser.Department;
                order.UserId = order.UserId == 0 ? currentUser.Id : order.UserId;
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
        [HttpPut("update/{id:long}")]
        public async Task<IActionResult> UpdateOrder(long id, [FromBody] Order order)
        {
            var currentUser = (User?)HttpContext.Items["User"];
            if (currentUser == null)
                return Unauthorized("無效使用者");

            var existing = await _orderService.GetOrderByIdAsync(id);
            if (existing == null)
                return NotFound("訂單不存在");

            // 權限檢查
            if (currentUser.Role == UserRole.Manager)
            {
                if (!string.Equals(existing.Department, currentUser.Department, StringComparison.OrdinalIgnoreCase))
                    return Forbid("沒有權限更新其他部門的訂單");
            }
            else if (existing.UserId != currentUser.Id)
            {
                return Forbid("沒有權限更新他人訂單");
            }

            // ✅ 共用驗證方法
            var validationError = ValidateOrder(order, false);
            if (validationError != null)
                return validationError;

            // 保留原本不可修改的屬性
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
        [HttpDelete("delete/{id:long}")]
        public async Task<IActionResult> DeleteOrder(long id)
        {
            var currentUser = (User?)HttpContext.Items["User"];
            if (currentUser == null)
                return Unauthorized("無效使用者");

            var existing = await _orderService.GetOrderByIdAsync(id);
            if (existing == null)
                return NotFound("訂單不存在");

            // 權限檢查
            if (currentUser.Role == UserRole.Manager)
            {
                if (!string.Equals(existing.Department, currentUser.Department, StringComparison.OrdinalIgnoreCase))
                    return Forbid("沒有權限刪除此訂單");
            }
            else if (existing.UserId != currentUser.Id)
            {
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

        // ---------- 共用驗證方法 ----------
        private IActionResult? ValidateOrder(Order order, bool isCreate)
        {
            if (isCreate && string.IsNullOrWhiteSpace(order.OrderNumber))
                return BadRequest("訂單編號為必填");

            if (string.IsNullOrWhiteSpace(order.ProductName))
                return BadRequest("產品名稱為必填");

            if (order.Quantity is null || order.Quantity <= 0)
                return BadRequest("數量必須大於 0");

            if (order.Price is null || order.Price < 0)
                return BadRequest("價格不可為負數");

            return null;
        }
    }
}
