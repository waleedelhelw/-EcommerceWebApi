using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.DTOs.Dashboard;
using EcommerceWepApi.BLL.Services.Interfaces;
using EcommerceWepApi.DAL.Models.Enums;
using EcommerceWepApi.DAL.Repositories.Interfaces;
using AutoMapper;

namespace EcommerceWepApi.BLL.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DashboardService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// جلب بيانات لوحة التحكم
        /// </summary>
        public async Task<ApiResponse<DashboardDto>> GetDashboardAsync()
        {
            var totalOrders = await _unitOfWork.Orders.CountAsync();
            var totalUsers = await _unitOfWork.Users.CountAsync(u => !u.IsDeleted);
            var totalProducts = await _unitOfWork.Products.CountAsync(p => !p.IsDeleted);
            var totalCategories = await _unitOfWork.Categories.CountAsync(c => !c.IsDeleted);

            var pendingOrders = await _unitOfWork.Orders.CountAsync(
                o => o.Status == OrderStatus.Pending);
            var deliveredOrders = await _unitOfWork.Orders.CountAsync(
                o => o.Status == OrderStatus.Delivered);

            // حساب إجمالي المبيعات
            var allOrders = await _unitOfWork.Orders.GetAllAsync(
                o => o.Status != OrderStatus.Cancelled);
            var totalSales = allOrders.Sum(o => o.TotalPrice);

            var deliveredOrdersList = await _unitOfWork.Orders.GetAllAsync(
                o => o.Status == OrderStatus.Delivered);
            var totalRevenue = deliveredOrdersList.Sum(o => o.TotalPrice);

            // المبيعات الشهرية (آخر 6 أشهر)
            var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
            var recentOrders = await _unitOfWork.Orders.GetAllAsync(
                o => o.CreatedAt >= sixMonthsAgo && o.Status != OrderStatus.Cancelled);

            var monthlySales = recentOrders
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new MonthlySalesDto
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Sales = g.Sum(o => o.TotalPrice),
                    OrderCount = g.Count()
                })
                .OrderBy(m => m.Month)
                .ToList();

            // آخر 10 طلبات
            var (latestOrders, _) = await _unitOfWork.Orders.GetPagedAsync(
                1, 10,
                orderBy: o => o.CreatedAt,
                isDescending: true,
                includes: o => o.User);

            var recentOrderDtos = _mapper.Map<List<RecentOrderDto>>(latestOrders);

            var dashboard = new DashboardDto
            {
                TotalOrders = totalOrders,
                TotalSales = totalSales,
                TotalRevenue = totalRevenue,
                TotalUsers = totalUsers,
                TotalProducts = totalProducts,
                TotalCategories = totalCategories,
                PendingOrders = pendingOrders,
                DeliveredOrders = deliveredOrders,
                MonthlySales = monthlySales,
                RecentOrders = recentOrderDtos
            };

            return ApiResponse<DashboardDto>.SuccessResponse(dashboard);
        }

        /// <summary>
        /// تقرير المبيعات
        /// </summary>
        public async Task<ApiResponse<SalesReportDto>> GetSalesReportAsync(
            DateTime startDate, DateTime endDate)
        {
            var orders = await _unitOfWork.Orders.GetAllAsync(
                o => o.CreatedAt >= startDate && o.CreatedAt <= endDate
                    && o.Status != OrderStatus.Cancelled);

            var orderList = orders.ToList();

            var dailySales = orderList
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new DailySalesDto
                {
                    Date = g.Key,
                    Sales = g.Sum(o => o.TotalPrice),
                    OrderCount = g.Count()
                })
                .OrderBy(d => d.Date)
                .ToList();

            var report = new SalesReportDto
            {
                TotalSales = orderList.Sum(o => o.TotalPrice),
                TotalOrders = orderList.Count,
                AverageOrderValue = orderList.Any() ? orderList.Average(o => o.TotalPrice) : 0,
                DailySales = dailySales
            };

            return ApiResponse<SalesReportDto>.SuccessResponse(report);
        }

        /// <summary>
        /// أكثر المنتجات مبيعاً
        /// </summary>
        public async Task<ApiResponse<List<TopProductDto>>> GetTopSellingProductsAsync(int count = 10)
        {
            var orderItems = await _unitOfWork.OrderItems.GetAllWithIncludeAsync(
                oi => oi.Order.Status == OrderStatus.Delivered,
                oi => oi.Product);

            var topProducts = orderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new TopProductDto
                {
                    Id = g.Key,
                    Name = g.First().Product.Name,
                    ImageUrl = g.First().Product.ImageUrl,
                    TotalSold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Quantity * oi.UnitPrice),
                    Rating = g.First().Product.Rating
                })
                .OrderByDescending(p => p.TotalSold)
                .Take(count)
                .ToList();

            return ApiResponse<List<TopProductDto>>.SuccessResponse(topProducts);
        }

        /// <summary>
        /// أكثر المنتجات تقييماً
        /// </summary>
        public async Task<ApiResponse<List<TopProductDto>>> GetTopRatedProductsAsync(int count = 10)
        {
            var (products, _) = await _unitOfWork.Products.GetPagedAsync(
                1, count,
                p => !p.IsDeleted && p.IsActive && p.TotalRatings > 0,
                p => p.Rating,
                true);

            var topRated = products.Select(p => new TopProductDto
            {
                Id = p.Id,
                Name = p.Name,
                ImageUrl = p.ImageUrl,
                TotalSold = 0,
                TotalRevenue = 0,
                Rating = p.Rating
            }).ToList();

            return ApiResponse<List<TopProductDto>>.SuccessResponse(topRated);
        }
    }
}