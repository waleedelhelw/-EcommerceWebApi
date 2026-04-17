using EcommerceWepApi.BLL.DTOs.Common;

namespace EcommerceWepApi.BLL.DTOs.Product
{
    /// <summary>
    /// فلاتر البحث والتصفية للمنتجات
    /// </summary>
    public class ProductFilterDto : PaginationParams
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinRating { get; set; }
        public string? SortBy { get; set; } // price, rating, newest, name
        public bool IsDescending { get; set; } = false;
    }
}