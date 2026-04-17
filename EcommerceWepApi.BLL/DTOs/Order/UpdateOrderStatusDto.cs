using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.BLL.DTOs.Order
{
    public class UpdateOrderStatusDto
    {
        [Required(ErrorMessage = "حالة الطلب مطلوبة")]
        public string Status { get; set; } = string.Empty;
    }
}