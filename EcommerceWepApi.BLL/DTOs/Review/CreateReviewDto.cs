using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.BLL.DTOs.Review
{
    public class CreateReviewDto
    {
        [Required(ErrorMessage = "المنتج مطلوب")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "التقييم مطلوب")]
        [Range(1, 5, ErrorMessage = "التقييم يجب أن يكون بين 1 و 5")]
        public int Rating { get; set; }

        [MaxLength(1000, ErrorMessage = "التعليق لا يتجاوز 1000 حرف")]
        public string? Comment { get; set; }

        [MaxLength(200, ErrorMessage = "العنوان لا يتجاوز 200 حرف")]
        public string? Title { get; set; }
    }
}