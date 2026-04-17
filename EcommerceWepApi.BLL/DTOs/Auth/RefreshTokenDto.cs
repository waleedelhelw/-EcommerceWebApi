using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.BLL.DTOs.Auth
{
    public class RefreshTokenDto
    {
        [Required(ErrorMessage = "التوكن مطلوب")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "الـ Refresh Token مطلوب")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}