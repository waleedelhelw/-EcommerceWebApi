using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.BLL.DTOs.Auth
{
    /// <summary>
    /// بيانات تسجيل مستخدم جديد
    /// </summary>
    public class RegisterDto
    {
        [Required(ErrorMessage = "الاسم مطلوب")]
        [MaxLength(100, ErrorMessage = "الاسم لا يتجاوز 100 حرف")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [MinLength(6, ErrorMessage = "كلمة المرور يجب أن تكون 6 أحرف على الأقل")]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيدها غير متطابقين")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        [MaxLength(20)]
        public string? Phone { get; set; }
    }
}