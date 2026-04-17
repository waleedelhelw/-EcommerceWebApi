using System.Linq.Expressions;

namespace EcommerceWepApi.DAL.Repositories.Interfaces
{
    /// <summary>
    /// الـ Repository العام - يحتوي على جميع العمليات الأساسية
    /// </summary>
    public interface IGenericRepository<T> where T : class
    {
        // جلب الكل
        Task<IEnumerable<T>> GetAllAsync();

        // جلب الكل مع تصفية
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);

        // جلب بالـ Id
        Task<T?> GetByIdAsync(int id);

        // جلب عنصر واحد بشرط
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);

        // جلب مع Include
        Task<IEnumerable<T>> GetAllWithIncludeAsync(
            Expression<Func<T, bool>>? predicate = null,
            params Expression<Func<T, object>>[] includes);

        // جلب عنصر واحد مع Include
        Task<T?> GetFirstWithIncludeAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);

        // Pagination - ترقيم الصفحات
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, object>>? orderBy = null,
            bool isDescending = false,
            params Expression<Func<T, object>>[] includes);

        // إضافة
        Task<T> AddAsync(T entity);

        // إضافة مجموعة
        Task AddRangeAsync(IEnumerable<T> entities);

        // تحديث
        void Update(T entity);

        // حذف
        void Delete(T entity);

        // حذف مجموعة
        void DeleteRange(IEnumerable<T> entities);

        // عدد العناصر
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

        // هل العنصر موجود؟
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    }
}