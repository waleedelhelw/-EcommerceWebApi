using EcommerceWepApi.DAL.Models;

namespace EcommerceWepApi.DAL.Repositories.Interfaces
{
    /// <summary>
    /// وحدة العمل - تجمع كل الـ Repositories في مكان واحد
    /// وتضمن حفظ جميع التغييرات في Transaction واحدة
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Product> Products { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Order> Orders { get; }
        IGenericRepository<OrderItem> OrderItems { get; }
        IGenericRepository<Cart> Carts { get; }
        IGenericRepository<Review> Reviews { get; }
        IGenericRepository<Payment> Payments { get; }
        IGenericRepository<AdminLog> AdminLogs { get; }
        IGenericRepository<Wishlist> Wishlists { get; }

        /// <summary>
        /// حفظ جميع التغييرات في قاعدة البيانات
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}