using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.BLL.DTOs.Cart
{
    public class UpdateCartDto
    {
        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(1, 100, ErrorMessage = "الكمية يجب أن تكون بين 1 و 100")]
        public int Quantity { get; set; }
    }
}