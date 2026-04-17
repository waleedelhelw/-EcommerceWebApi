namespace EcommerceWepApi.BLL.DTOs.Wishlist
{
    public class WishlistDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImageUrl { get; set; }
        public decimal ProductPrice { get; set; }
        public bool IsInStock { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}