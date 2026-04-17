using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.DTOs.Cart;

namespace EcommerceWepApi.BLL.Services.Interfaces
{
    /// <summary>
    /// خدمة سلة التسوق
    /// </summary>
    public interface ICartService
    {
        // جلب سلة المستخدم
        Task<ApiResponse<CartSummaryDto>> GetCartAsync(int userId);

        // إضافة منتج للسلة
        Task<ApiResponse<CartDto>> AddToCartAsync(int userId, AddToCartDto dto);

        // تحديث كمية منتج في السلة
        Task<ApiResponse<CartDto>> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartDto dto);

        // إزالة منتج من السلة
        Task<ApiResponse<bool>> RemoveFromCartAsync(int userId, int cartItemId);

        // مسح السلة بالكامل
        Task<ApiResponse<bool>> ClearCartAsync(int userId);
    }
}