using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.DTOs.Order;
using EcommerceWepApi.BLL.Services.Interfaces;

namespace EcommerceWepApi.API.Controllers.Customer
{
    [Route("api/customer/orders")]
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class CustomerOrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public CustomerOrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// إنشاء طلب جديد
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var userId = GetUserId();
            var result = await _orderService.CreateOrderAsync(userId, dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// جلب طلباتي
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMyOrders([FromQuery] PaginationParams paginationParams)
        {
            var userId = GetUserId();
            var result = await _orderService.GetUserOrdersAsync(userId, paginationParams);
            return Ok(result);
        }

        /// <summary>
        /// جلب تفاصيل طلب معين
        /// </summary>
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var userId = GetUserId();
            var result = await _orderService.GetOrderByIdAsync(orderId, userId, isAdmin: false);
            return Ok(result);
        }

        /// <summary>
        /// إلغاء طلب
        /// </summary>
        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var userId = GetUserId();
            var result = await _orderService.CancelOrderAsync(orderId, userId);
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