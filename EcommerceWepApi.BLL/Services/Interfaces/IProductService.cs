using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.DTOs.Product;

namespace EcommerceWepApi.BLL.Services.Interfaces
{
    /// <summary>
    /// خدمة إدارة المنتجات
    /// </summary>
    public interface IProductService
    {
        // جلب المنتجات مع فلاتر وترقيم صفحات
        Task<ApiResponse<PaginatedResponse<ProductDto>>> GetAllProductsAsync(ProductFilterDto filter);

        // جلب منتج بالمعرّف
        Task<ApiResponse<ProductDto>> GetProductByIdAsync(int id);

        // جلب المنتجات حسب الفئة
        Task<ApiResponse<List<ProductDto>>> GetProductsByCategoryAsync(int categoryId);

        // جلب المنتجات المميزة (الأعلى تقييماً)
        Task<ApiResponse<List<ProductDto>>> GetFeaturedProductsAsync(int count = 10);

        // جلب أحدث المنتجات
        Task<ApiResponse<List<ProductDto>>> GetNewProductsAsync(int count = 10);

        // جلب المنتجات ذات الصلة
        Task<ApiResponse<List<ProductDto>>> GetRelatedProductsAsync(int productId, int count = 5);

        // إنشاء منتج جديد (أدمن)
        Task<ApiResponse<ProductDto>> CreateProductAsync(CreateProductDto dto, int adminId);

        // تحديث منتج (أدمن)
        Task<ApiResponse<ProductDto>> UpdateProductAsync(int id, UpdateProductDto dto, int adminId);

        // حذف منتج - Soft Delete (أدمن)
        Task<ApiResponse<bool>> DeleteProductAsync(int id, int adminId);
    }
}