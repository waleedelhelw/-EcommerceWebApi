using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.DTOs.User;

namespace EcommerceWepApi.BLL.Services.Interfaces
{
    /// <summary>
    /// خدمة إدارة المستخدمين
    /// </summary>
    public interface IUserService
    {
        // جلب جميع المستخدمين مع ترقيم الصفحات (أدمن)
        Task<ApiResponse<PaginatedResponse<UserDto>>> GetAllUsersAsync(PaginationParams paginationParams);

        // جلب مستخدم بالمعرّف
        Task<ApiResponse<UserDto>> GetUserByIdAsync(int id);

        // جلب الملف الشخصي للمستخدم الحالي
        Task<ApiResponse<UserDto>> GetProfileAsync(int userId);

        // تحديث بيانات المستخدم
        Task<ApiResponse<UserDto>> UpdateUserAsync(int userId, UpdateUserDto dto);

        // تفعيل / حظر مستخدم (أدمن)
        Task<ApiResponse<bool>> ToggleUserStatusAsync(int userId, int adminId);

        // حذف مستخدم - Soft Delete (أدمن)
        Task<ApiResponse<bool>> DeleteUserAsync(int userId, int adminId);
    }
}