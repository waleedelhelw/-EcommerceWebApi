using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.DTOs.Dashboard;

namespace EcommerceWepApi.BLL.Services.Interfaces
{
    /// <summary>
    /// خدمة لوحة التحكم والتقارير
    /// </summary>
    public interface IDashboardService
    {
        // جلب بيانات لوحة التحكم
        Task<ApiResponse<DashboardDto>> GetDashboardAsync();

        // تقرير المبيعات
        Task<ApiResponse<SalesReportDto>> GetSalesReportAsync(DateTime startDate, DateTime endDate);

        // أكثر المنتجات مبيعاً
        Task<ApiResponse<List<TopProductDto>>> GetTopSellingProductsAsync(int count = 10);

        // أكثر المنتجات تقييماً
        Task<ApiResponse<List<TopProductDto>>> GetTopRatedProductsAsync(int count = 10);
    }
}