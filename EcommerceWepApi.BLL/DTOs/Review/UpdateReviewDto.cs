using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.BLL.DTOs.Review
{
    public class UpdateReviewDto
    {
        [Required(ErrorMessage = "التقييم مطلوب")]
        [Range(1, 5, ErrorMessage = "التقييم يجب أن يكون بين 1 و 5")]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        [MaxLength(200)]
        public string? Title { get; set; }
    }
}