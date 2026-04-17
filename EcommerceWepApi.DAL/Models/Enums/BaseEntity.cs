namespace EcommerceWepApi.DAL.Models
{
    /// <summary>
    /// الكلاس الأساسي لجميع الكيانات - يحتوي على الحقول المشتركة
    /// </summary>
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}