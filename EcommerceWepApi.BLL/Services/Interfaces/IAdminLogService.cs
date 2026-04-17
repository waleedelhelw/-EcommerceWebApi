using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.DTOs.AdminLog;

namespace EcommerceWepApi.BLL.Services.Interfaces
{
    /// <summary>
    /// خدمة سجلات الأدمن
    /// </summary>
    public interface IAdminLogService
    {
        // تسجيل عملية
        Task LogActionAsync(int adminId, string action, string? entityName = null,
            int? entityId = null, string? oldValues = null, string? newValues = null,
            string? details = null);

        // جلب السجلات
        Task<ApiResponse<PaginatedResponse<AdminLogDto>>> GetLogsAsync(PaginationParams paginationParams);
    }
}