using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.DTOs.Wishlist;

namespace EcommerceWepApi.BLL.Services.Interfaces
{
    /// <summary>
    /// خدمة المفضلة
    /// </summary>
    public interface IWishlistService
    {
        // جلب المفضلة
        Task<ApiResponse<List<WishlistDto>>> GetWishlistAsync(int userId);

        // إضافة منتج للمفضلة
        Task<ApiResponse<WishlistDto>> AddToWishlistAsync(int userId, int productId);

        // إزالة منتج من المفضلة
        Task<ApiResponse<bool>> RemoveFromWishlistAsync(int userId, int wishlistId);
    }
}