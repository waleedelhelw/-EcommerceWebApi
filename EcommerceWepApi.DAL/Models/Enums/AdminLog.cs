using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.DAL.Models
{
    /// <summary>
    /// جدول سجلات الأدمن - تسجيل جميع العمليات
    /// </summary>
    public class AdminLog
    {
        public int Id { get; set; }

        public int AdminId { get; set; }
        public virtual User Admin { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Action { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? EntityName { get; set; }

        public int? EntityId { get; set; }

        public string? OldValues { get; set; }
        public string? NewValues { get; set; }

        [MaxLength(500)]
        public string? Details { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}