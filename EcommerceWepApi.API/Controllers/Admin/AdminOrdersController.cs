using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.DTOs.Order;
using EcommerceWepApi.BLL.Services.Interfaces;

namespace EcommerceWepApi.API.Controllers.Admin
{
    [Route("api/admin/orders")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminOrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public AdminOrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// جلب جميع الطلبات
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] PaginationParams paginationParams,
            [FromQuery] string? status = null)
        {
            var result = await _orderService.GetAllOrdersAsync(paginationParams, status);
            return Ok(result);
        }

        /// <summary>
        /// جلب تفاصيل طلب معين
        /// </summary>
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var adminId = GetUserId();
            var result = await _orderService.GetOrderByIdAsync(orderId, adminId, isAdmin: true);
            return Ok(result);
        }

        /// <summary>
        /// تحديث حالة طلب
        /// </summary>
        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto dto)
        {
            var adminId = GetUserId();
            var result = await _orderService.UpdateOrderStatusAsync(orderId, dto, adminId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}