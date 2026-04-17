using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.BLL.DTOs.Cart
{
    public class AddToCartDto
    {
        [Required(ErrorMessage = "المنتج مطلوب")]
        public int ProductId { get; set; }

        [Range(1, 100, ErrorMessage = "الكمية يجب أن تكون بين 1 و 100")]
        public int Quantity { get; set; } = 1;
    }
}