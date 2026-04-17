using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceWepApi.BLL.DTOs.Review;
using EcommerceWepApi.BLL.Services.Interfaces;

namespace EcommerceWepApi.API.Controllers.Customer
{
    [Route("api/customer/reviews")]
    [ApiController]
    public class CustomerReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public CustomerReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// جلب تقييمات منتج معين
        /// </summary>
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var result = await _reviewService.GetProductReviewsAsync(productId);
            return Ok(result);
        }

        /// <summary>
        /// إضافة تقييم لمنتج
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
        {
            var userId = GetUserId();
            var result = await _reviewService.CreateReviewAsync(userId, dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// تعديل تقييم
        /// </summary>
        [HttpPut("{reviewId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] UpdateReviewDto dto)
        {
            var userId = GetUserId();
            var result = await _reviewService.UpdateReviewAsync(userId, reviewId, dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// حذف تقييم
        /// </summary>
        [HttpDelete("{reviewId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var userId = GetUserId();
            var result = await _reviewService.DeleteReviewAsync(userId, reviewId, isAdmin: false);
            return Ok(result);
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}