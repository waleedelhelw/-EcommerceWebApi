using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWepApi.DAL.Models
{
    /// <summary>
    /// جدول المنتجات
    /// </summary>
    public class Product : BaseEntity
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        // المفتاح الأجنبي للفئة
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal Rating { get; set; } = 0;

        public int TotalRatings { get; set; } = 0;

        // الأدمن الذي أنشأ المنتج
        public int CreatedById { get; set; }
        public virtual User CreatedBy { get; set; } = null!;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        // العلاقات
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<Cart> CartItems { get; set; } = new List<Cart>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Wishlist> WishlistItems { get; set; } = new List<Wishlist>();
    }
}