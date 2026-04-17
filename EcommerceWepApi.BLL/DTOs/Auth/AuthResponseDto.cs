namespace EcommerceWepApi.BLL.DTOs.Auth
{
    /// <summary>
    /// استجابة المصادقة - تحتوي على التوكن وبيانات المستخدم
    /// </summary>
    public class AuthResponseDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenExpiration { get; set; }
    }
}