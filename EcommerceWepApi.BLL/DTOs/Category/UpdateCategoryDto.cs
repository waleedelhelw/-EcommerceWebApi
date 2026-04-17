using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.BLL.DTOs.Category
{
    public class UpdateCategoryDto
    {
        [Required(ErrorMessage = "اسم الفئة مطلوب")]
        [MaxLength(100, ErrorMessage = "اسم الفئة لا يتجاوز 100 حرف")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "الوصف لا يتجاوز 500 حرف")]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; }
    }
}