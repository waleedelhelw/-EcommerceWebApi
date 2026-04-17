namespace EcommerceWepApi.BLL.DTOs.AdminLog
{
    public class AdminLogDto
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public string AdminName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? EntityName { get; set; }
        public int? EntityId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? Details { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
