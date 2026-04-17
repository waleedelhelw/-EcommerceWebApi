using EcommerceWepApi.DAL.Data;
using EcommerceWepApi.DAL.Models;
using EcommerceWepApi.DAL.Repositories.Interfaces;

namespace EcommerceWepApi.DAL.Repositories.Implementations
{
    /// <summary>
    /// تطبيق وحدة العمل
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        // الـ Repositories - Lazy Loading
        private IGenericRepository<User>? _users;
        private IGenericRepository<Product>? _products;
        private IGenericRepository<Category>? _categories;
        private IGenericRepository<Order>? _orders;
        private IGenericRepository<OrderItem>? _orderItems;
        private IGenericRepository<Cart>? _carts;
        private IGenericRepository<Review>? _reviews;
        private IGenericRepository<Payment>? _payments;
        private IGenericRepository<AdminLog>? _adminLogs;
        private IGenericRepository<Wishlist>? _wishlists;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========== خصائص الـ Repositories ==========
        public IGenericRepository<User> Users =>
            _users ??= new GenericRepository<User>(_context);

        public IGenericRepository<Product> Products =>
            _products ??= new GenericRepository<Product>(_context);

        public IGenericRepository<Category> Categories =>
            _categories ??= new GenericRepository<Category>(_context);

        public IGenericRepository<Order> Orders =>
            _orders ??= new GenericRepository<Order>(_context);

        public IGenericRepository<OrderItem> OrderItems =>
            _orderItems ??= new GenericRepository<OrderItem>(_context);

        public IGenericRepository<Cart> Carts =>
            _carts ??= new GenericRepository<Cart>(_context);

        public IGenericRepository<Review> Reviews =>
            _reviews ??= new GenericRepository<Review>(_context);

        public IGenericRepository<Payment> Payments =>
            _payments ??= new GenericRepository<Payment>(_context);

        public IGenericRepository<AdminLog> AdminLogs =>
            _adminLogs ??= new GenericRepository<AdminLog>(_context);

        public IGenericRepository<Wishlist> Wishlists =>
            _wishlists ??= new GenericRepository<Wishlist>(_context);

        // ========== حفظ التغييرات ==========
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // ========== تنظيف الموارد ==========
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}