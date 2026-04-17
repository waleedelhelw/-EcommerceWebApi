using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceWepApi.BLL.Services.Interfaces;

namespace EcommerceWepApi.API.Controllers.Admin
{
    [Route("api/admin/dashboard")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public AdminDashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// جلب بيانات لوحة التحكم
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _dashboardService.GetDashboardAsync();
            return Ok(result);
        }

        /// <summary>
        /// تقرير المبيعات
        /// </summary>
        [HttpGet("sales-report")]
        public async Task<IActionResult> GetSalesReport(
            [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _dashboardService.GetSalesReportAsync(startDate, endDate);
            return Ok(result);
        }

        /// <summary>
        /// أكثر المنتجات مبيعاً
        /// </summary>
        [HttpGet("top-selling")]
        public async Task<IActionResult> GetTopSellingProducts([FromQuery] int count = 10)
        {
            var result = await _dashboardService.GetTopSellingProductsAsync(count);
            return Ok(result);
        }

        /// <summary>
        /// أكثر المنتجات تقييماً
        /// </summary>
        [HttpGet("top-rated")]
        public async Task<IActionResult> GetTopRatedProducts([FromQuery] int count = 10)
        {
            var result = await _dashboardService.GetTopRatedProductsAsync(count);
            return Ok(result);
        }
    }
}