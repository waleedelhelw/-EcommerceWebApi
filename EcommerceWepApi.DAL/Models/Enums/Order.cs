using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EcommerceWepApi.DAL.Models.Enums;

namespace EcommerceWepApi.DAL.Models
{
    /// <summary>
    /// جدول الطلبات
    /// </summary>
    public class Order : BaseEntity
    {
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [MaxLength(500)]
        public string? ShippingAddress { get; set; }

        [MaxLength(100)]
        public string? ShippingCity { get; set; }

        [MaxLength(100)]
        public string? ShippingCountry { get; set; }

        [MaxLength(1000)]
        public string? OrderNotes { get; set; }

        // العلاقات
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual Payment? Payment { get; set; }
    }
}