using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.DTOs.Order;

namespace EcommerceWepApi.BLL.Services.Interfaces
{
    /// <summary>
    /// خدمة إدارة الطلبات
    /// </summary>
    public interface IOrderService
    {
        // إنشاء طلب جديد من السلة
        Task<ApiResponse<OrderDto>> CreateOrderAsync(int userId, CreateOrderDto dto);

        // جلب طلبات المستخدم
        Task<ApiResponse<PaginatedResponse<OrderDto>>> GetUserOrdersAsync(int userId, PaginationParams paginationParams);

        // جلب تفاصيل طلب معين
        Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int orderId, int userId, bool isAdmin = false);

        // جلب جميع الطلبات (أدمن)
        Task<ApiResponse<PaginatedResponse<OrderDto>>> GetAllOrdersAsync(PaginationParams paginationParams, string? statusFilter = null);

        // تحديث حالة الطلب (أدمن)
        Task<ApiResponse<OrderDto>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto, int adminId);

        // إلغاء طلب (المستخدم - قبل الشحن فقط)
        Task<ApiResponse<bool>> CancelOrderAsync(int orderId, int userId);
    }
}