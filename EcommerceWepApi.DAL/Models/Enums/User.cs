using System.ComponentModel.DataAnnotations;
using EcommerceWepApi.DAL.Models.Enums;

namespace EcommerceWepApi.DAL.Models
{
    /// <summary>
    /// جدول المستخدمين - يشمل الأدمن والعملاء
    /// </summary>
    public class User : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        public UserRole Role { get; set; } = UserRole.Customer;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        // حقول الـ Refresh Token
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // العلاقات
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Cart> CartItems { get; set; } = new List<Cart>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Wishlist> WishlistItems { get; set; } = new List<Wishlist>();
        public virtual ICollection<Product> CreatedProducts { get; set; } = new List<Product>();
        public virtual ICollection<AdminLog> AdminLogs { get; set; } = new List<AdminLog>();
    }
}