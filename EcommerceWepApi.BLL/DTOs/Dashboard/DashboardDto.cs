namespace EcommerceWepApi.BLL.DTOs.Dashboard
{
    public class DashboardDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int PendingOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public List<MonthlySalesDto> MonthlySales { get; set; } = new();
        public List<RecentOrderDto> RecentOrders { get; set; } = new();
    }

    public class MonthlySalesDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal Sales { get; set; }
        public int OrderCount { get; set; }
    }

    public class RecentOrderDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class TopProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal Rating { get; set; }
    }

    public class SalesReportDto
    {
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<DailySalesDto> DailySales { get; set; } = new();
    }

    public class DailySalesDto
    {
        public DateTime Date { get; set; }
        public decimal Sales { get; set; }
        public int OrderCount { get; set; }
    }
}