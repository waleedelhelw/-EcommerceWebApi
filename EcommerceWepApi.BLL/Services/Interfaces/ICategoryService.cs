using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.DTOs.Category;

namespace EcommerceWepApi.BLL.Services.Interfaces
{
    /// <summary>
    /// خدمة إدارة الفئات
    /// </summary>
    public interface ICategoryService
    {
        // جلب جميع الفئات
        Task<ApiResponse<List<CategoryDto>>> GetAllCategoriesAsync(bool includeInactive = false);

        // جلب فئة بالمعرّف
        Task<ApiResponse<CategoryDto>> GetCategoryByIdAsync(int id);

        // إنشاء فئة جديدة
        Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto, int adminId);

        // تحديث فئة
        Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto dto, int adminId);

        // حذف فئة - Soft Delete
        Task<ApiResponse<bool>> DeleteCategoryAsync(int id, int adminId);
    }
}