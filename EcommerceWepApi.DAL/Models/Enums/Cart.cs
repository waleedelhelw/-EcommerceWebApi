namespace EcommerceWepApi.DAL.Models
{
    /// <summary>
    /// جدول سلة التسوق
    /// </summary>
    public class Cart : BaseEntity
    {
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        public int Quantity { get; set; } = 1;
    }
}