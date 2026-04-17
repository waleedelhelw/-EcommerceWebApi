using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.DAL.Models
{
    /// <summary>
    /// جدول التقييمات والمراجعات
    /// </summary>
    public class Review : BaseEntity
    {
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        public int Rating { get; set; } // من 1 إلى 5

        [MaxLength(1000)]
        public string? Comment { get; set; }

        [MaxLength(200)]
        public string? Title { get; set; }

        public bool IsApproved { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
    }
}