using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.BLL.DTOs.Product
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        [MaxLength(200, ErrorMessage = "اسم المنتج لا يتجاوز 200 حرف")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000, ErrorMessage = "الوصف لا يتجاوز 2000 حرف")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "السعر مطلوب")]
        [Range(0.01, double.MaxValue, ErrorMessage = "السعر يجب أن يكون أكبر من صفر")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(0, int.MaxValue, ErrorMessage = "الكمية لا يمكن أن تكون سالبة")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "الفئة مطلوبة")]
        public int CategoryId { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }
    }
}