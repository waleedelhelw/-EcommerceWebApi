using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.BLL.DTOs.User
{
    /// <summary>
    /// تحديث بيانات المستخدم
    /// </summary>
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "الاسم مطلوب")]
        [MaxLength(100, ErrorMessage = "الاسم لا يتجاوز 100 حرف")]
        public string Name { get; set; } = string.Empty;

        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }
    }
}