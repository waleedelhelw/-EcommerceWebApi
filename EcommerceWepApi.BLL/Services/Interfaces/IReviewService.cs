using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.DTOs.Review;

namespace EcommerceWepApi.BLL.Services.Interfaces
{
    /// <summary>
    /// خدمة التقييمات والمراجعات
    /// </summary>
    public interface IReviewService
    {
        // جلب تقييمات منتج معين
        Task<ApiResponse<List<ReviewDto>>> GetProductReviewsAsync(int productId);

        // جلب جميع التقييمات (أدمن)
        Task<ApiResponse<PaginatedResponse<ReviewDto>>> GetAllReviewsAsync(PaginationParams paginationParams);

        // إضافة تقييم (بعد الشراء فقط)
        Task<ApiResponse<ReviewDto>> CreateReviewAsync(int userId, CreateReviewDto dto);

        // تعديل تقييم
        Task<ApiResponse<ReviewDto>> UpdateReviewAsync(int userId, int reviewId, UpdateReviewDto dto);

        // حذف تقييم
        Task<ApiResponse<bool>> DeleteReviewAsync(int userId, int reviewId, bool isAdmin = false);

        // الموافقة على تقييم (أدمن)
        Task<ApiResponse<bool>> ApproveReviewAsync(int reviewId, int adminId);
    }
}