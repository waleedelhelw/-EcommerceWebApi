using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EcommerceWepApi.DAL.Models.Enums;

namespace EcommerceWepApi.DAL.Models
{
    /// <summary>
    /// جدول المدفوعات
    /// </summary>
    public class Payment : BaseEntity
    {
        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public PaymentMethod PaymentMethod { get; set; }

        [MaxLength(200)]
        public string? TransactionId { get; set; }

        [MaxLength(200)]
        public string? Reference { get; set; }
    }
}