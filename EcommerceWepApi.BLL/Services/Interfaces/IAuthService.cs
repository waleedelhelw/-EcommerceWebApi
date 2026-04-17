using EcommerceWepApi.BLL.DTOs.Auth;
using EcommerceWepApi.BLL.DTOs.Common;

namespace EcommerceWepApi.BLL.Services.Interfaces
{
    /// <summary>
    /// خدمة المصادقة والتسجيل
    /// </summary>
    public interface IAuthService
    {
        // تسجيل مستخدم جديد
        Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto);

        // تسجيل دخول
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto);

        // تحديث التوكن
        Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto);

        // تغيير كلمة المرور
        Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto dto);

        // تسجيل خروج
        Task<ApiResponse<bool>> LogoutAsync(int userId);
    }
}