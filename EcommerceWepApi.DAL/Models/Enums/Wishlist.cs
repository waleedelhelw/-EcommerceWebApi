namespace EcommerceWepApi.DAL.Models
{
    /// <summary>
    /// جدول المفضلة
    /// </summary>
    public class Wishlist
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}