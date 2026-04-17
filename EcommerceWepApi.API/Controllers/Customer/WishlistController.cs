using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceWepApi.BLL.Services.Interfaces;

namespace EcommerceWepApi.API.Controllers.Customer
{
    [Route("api/customer/wishlist")]
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        /// <summary>
        /// جلب المفضلة
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var userId = GetUserId();
            var result = await _wishlistService.GetWishlistAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// إضافة منتج للمفضلة
        /// </summary>
        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var userId = GetUserId();
            var result = await _wishlistService.AddToWishlistAsync(userId, productId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// إزالة منتج من المفضلة
        /// </summary>
        [HttpDelete("{wishlistId}")]
        public async Task<IActionResult> RemoveFromWishlist(int wishlistId)
        {
            var userId = GetUserId();
            var result = await _wishlistService.RemoveFromWishlistAsync(userId, wishlistId);
            return Ok(result);
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}