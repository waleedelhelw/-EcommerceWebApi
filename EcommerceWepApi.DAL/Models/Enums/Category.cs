using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.DAL.Models
{
    /// <summary>
    /// جدول الفئات / التصنيفات
    /// </summary>
    public class Category : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        // العلاقات
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}