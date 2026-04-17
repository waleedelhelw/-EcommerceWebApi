using AutoMapper;
using Microsoft.Extensions.Configuration;
using EcommerceWepApi.BLL.DTOs.Auth;
using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.Exceptions;
using EcommerceWepApi.BLL.Helpers;
using EcommerceWepApi.BLL.Services.Interfaces;
using EcommerceWepApi.DAL.Models;
using EcommerceWepApi.DAL.Models.Enums;
using EcommerceWepApi.DAL.Repositories.Interfaces;

namespace EcommerceWepApi.BLL.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtHelper _jwtHelper;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, JwtHelper jwtHelper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _jwtHelper = jwtHelper;
            _configuration = configuration;
        }

        /// <summary>
        /// تسجيل مستخدم جديد
        /// </summary>
        public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto)
        {
            // التحقق من عدم وجود بريد إلكتروني مكرر
            var existingUser = await _unitOfWork.Users.FindAsync(u => u.Email == dto.Email);
            if (existingUser != null)
            {
                return ApiResponse<AuthResponseDto>.FailureResponse("البريد الإلكتروني مسجل بالفعل");
            }

            // إنشاء المستخدم الجديد
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Phone = dto.Phone,
                Role = UserRole.Customer,
                IsActive = true,
                IsDeleted = false
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // إنشاء التوكن
            var token = _jwtHelper.GenerateToken(user);
            var refreshToken = _jwtHelper.GenerateRefreshToken();

            // حفظ الـ Refresh Token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                double.Parse(_configuration["JWT:RefreshTokenExpirationInDays"]!));
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var response = new AuthResponseDto
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiration = DateTime.UtcNow.AddHours(
                    double.Parse(_configuration["JWT:ExpirationInHours"]!))
            };

            return ApiResponse<AuthResponseDto>.SuccessResponse(response, "تم التسجيل بنجاح");
        }

        /// <summary>
        /// تسجيل الدخول
        /// </summary>
        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            // البحث عن المستخدم
            var user = await _unitOfWork.Users.FindAsync(u => u.Email == dto.Email && !u.IsDeleted);

            if (user == null)
            {
                return ApiResponse<AuthResponseDto>.FailureResponse("البريد الإلكتروني أو كلمة المرور غير صحيحة");
            }

            // التحقق من حالة الحساب
            if (!user.IsActive)
            {
                return ApiResponse<AuthResponseDto>.FailureResponse("الحساب محظور. تواصل مع الدعم الفني");
            }

            // التحقق من كلمة المرور
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                return ApiResponse<AuthResponseDto>.FailureResponse("البريد الإلكتروني أو كلمة المرور غير صحيحة");
            }

            // إنشاء التوكن
            var token = _jwtHelper.GenerateToken(user);
            var refreshToken = _jwtHelper.GenerateRefreshToken();

            // حفظ الـ Refresh Token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                double.Parse(_configuration["JWT:RefreshTokenExpirationInDays"]!));
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var response = new AuthResponseDto
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiration = DateTime.UtcNow.AddHours(
                    double.Parse(_configuration["JWT:ExpirationInHours"]!))
            };

            return ApiResponse<AuthResponseDto>.SuccessResponse(response, "تم تسجيل الدخول بنجاح");
        }

        /// <summary>
        /// تحديث التوكن باستخدام Refresh Token
        /// </summary>
        public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto)
        {
            // استخراج البيانات من التوكن المنتهي
            var principal = _jwtHelper.GetPrincipalFromExpiredToken(dto.Token);
            if (principal == null)
            {
                return ApiResponse<AuthResponseDto>.FailureResponse("التوكن غير صالح");
            }

            var userId = _jwtHelper.GetUserIdFromToken(principal);
            if (userId == null)
            {
                return ApiResponse<AuthResponseDto>.FailureResponse("التوكن غير صالح");
            }

            // البحث عن المستخدم
            var user = await _unitOfWork.Users.GetByIdAsync(userId.Value);
            if (user == null || user.IsDeleted || !user.IsActive)
            {
                return ApiResponse<AuthResponseDto>.FailureResponse("المستخدم غير موجود أو محظور");
            }

            // التحقق من صلاحية الـ Refresh Token
            if (user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return ApiResponse<AuthResponseDto>.FailureResponse("الـ Refresh Token منتهي أو غير صالح");
            }

            // إنشاء توكن جديد
            var newToken = _jwtHelper.GenerateToken(user);
            var newRefreshToken = _jwtHelper.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                double.Parse(_configuration["JWT:RefreshTokenExpirationInDays"]!));
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var response = new AuthResponseDto
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = newToken,
                RefreshToken = newRefreshToken,
                TokenExpiration = DateTime.UtcNow.AddHours(
                    double.Parse(_configuration["JWT:ExpirationInHours"]!))
            };

            return ApiResponse<AuthResponseDto>.SuccessResponse(response, "تم تحديث التوكن بنجاح");
        }

        /// <summary>
        /// تغيير كلمة المرور
        /// </summary>
        public async Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null || user.IsDeleted)
            {
                throw new NotFoundException("المستخدم", userId);
            }

            // التحقق من كلمة المرور الحالية
            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.Password))
            {
                return ApiResponse<bool>.FailureResponse("كلمة المرور الحالية غير صحيحة");
            }

            // تحديث كلمة المرور
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "تم تغيير كلمة المرور بنجاح");
        }

        /// <summary>
        /// تسجيل الخروج - إلغاء الـ Refresh Token
        /// </summary>
        public async Task<ApiResponse<bool>> LogoutAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("المستخدم", userId);
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "تم تسجيل الخروج بنجاح");
        }
    }
}