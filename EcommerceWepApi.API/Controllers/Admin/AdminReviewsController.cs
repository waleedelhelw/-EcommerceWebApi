using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.Services.Interfaces;

namespace EcommerceWepApi.API.Controllers.Admin
{
    [Route("api/admin/reviews")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public AdminReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// جلب جميع التقييمات
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllReviews([FromQuery] PaginationParams paginationParams)
        {
            var result = await _reviewService.GetAllReviewsAsync(paginationParams);
            return Ok(result);
        }

        /// <summary>
        /// الموافقة على تقييم
        /// </summary>
        [HttpPut("{reviewId}/approve")]
        public async Task<IActionResult> ApproveReview(int reviewId)
        {
            var adminId = GetUserId();
            var result = await _reviewService.ApproveReviewAsync(reviewId, adminId);
            return Ok(result);
        }

        /// <summary>
        /// حذف تقييم
        /// </summary>
        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var adminId = GetUserId();
            var result = await _reviewService.DeleteReviewAsync(adminId, reviewId, isAdmin: true);
            return Ok(result);
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}