using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceWepApi.BLL.DTOs.Cart;
using EcommerceWepApi.BLL.Services.Interfaces;

namespace EcommerceWepApi.API.Controllers.Customer
{
    [Route("api/customer/cart")]
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// جلب سلة التسوق
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            var result = await _cartService.GetCartAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// إضافة منتج للسلة
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var userId = GetUserId();
            var result = await _cartService.AddToCartAsync(userId, dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// تحديث كمية منتج في السلة
        /// </summary>
        [HttpPut("{cartItemId}")]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, [FromBody] UpdateCartDto dto)
        {
            var userId = GetUserId();
            var result = await _cartService.UpdateCartItemAsync(userId, cartItemId, dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// إزالة منتج من السلة
        /// </summary>
        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = GetUserId();
            var result = await _cartService.RemoveFromCartAsync(userId, cartItemId);
            return Ok(result);
        }

        /// <summary>
        /// مسح السلة بالكامل
        /// </summary>
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            var result = await _cartService.ClearCartAsync(userId);
            return Ok(result);
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}