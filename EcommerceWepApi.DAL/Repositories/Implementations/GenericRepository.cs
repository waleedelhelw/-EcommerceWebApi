using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using EcommerceWepApi.DAL.Data;
using EcommerceWepApi.DAL.Repositories.Interfaces;

namespace EcommerceWepApi.DAL.Repositories.Implementations
{
    /// <summary>
    /// تطبيق الـ Repository العام
    /// </summary>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // ========== جلب الكل ==========
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // ========== جلب الكل مع تصفية ==========
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        // ========== جلب بالـ Id ==========
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        // ========== جلب عنصر واحد بشرط ==========
        public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        // ========== جلب مع Include ==========
        public async Task<IEnumerable<T>> GetAllWithIncludeAsync(
            Expression<Func<T, bool>>? predicate = null,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // إضافة الـ Includes
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // إضافة الشرط إن وُجد
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.ToListAsync();
        }

        // ========== جلب عنصر واحد مع Include ==========
        public async Task<T?> GetFirstWithIncludeAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        // ========== Pagination - ترقيم الصفحات ==========
        public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, object>>? orderBy = null,
            bool isDescending = false,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // إضافة الـ Includes
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // إضافة الشرط إن وُجد
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            // عدد العناصر الكلي
            var totalCount = await query.CountAsync();

            // الترتيب
            if (orderBy != null)
            {
                query = isDescending
                    ? query.OrderByDescending(orderBy)
                    : query.OrderBy(orderBy);
            }

            // تطبيق الـ Pagination
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        // ========== إضافة ==========
        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        // ========== إضافة مجموعة ==========
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        // ========== تحديث ==========
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        // ========== حذف ==========
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        // ========== حذف مجموعة ==========
        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        // ========== عدد العناصر ==========
        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate == null
                ? await _dbSet.CountAsync()
                : await _dbSet.CountAsync(predicate);
        }

        // ========== هل العنصر موجود؟ ==========
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
}