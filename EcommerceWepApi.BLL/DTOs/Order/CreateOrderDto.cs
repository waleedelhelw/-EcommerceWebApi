using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.BLL.DTOs.Order
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "عنوان الشحن مطلوب")]
        [MaxLength(500)]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "المدينة مطلوبة")]
        [MaxLength(100)]
        public string ShippingCity { get; set; } = string.Empty;

        [Required(ErrorMessage = "الدولة مطلوبة")]
        [MaxLength(100)]
        public string ShippingCountry { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? OrderNotes { get; set; }

        // طريقة الدفع
        [Required(ErrorMessage = "طريقة الدفع مطلوبة")]
        public string PaymentMethod { get; set; } = string.Empty;
    }
}