using Microsoft.EntityFrameworkCore;
using EcommerceWepApi.DAL.Models;

namespace EcommerceWepApi.DAL.Data
{
    /// <summary>
    /// سياق قاعدة البيانات الرئيسي
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // الجداول
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<AdminLog> AdminLogs { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========== إعدادات جدول المستخدمين ==========
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Role).HasConversion<string>();
            });

            // ========== إعدادات جدول المنتجات ==========
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.CreatedBy)
                    .WithMany(u => u.CreatedProducts)
                    .HasForeignKey(p => p.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(p => p.Name);
                entity.HasIndex(p => p.Price);
            });

            // ========== إعدادات جدول الطلبات ==========
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(o => o.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(o => o.Status).HasConversion<string>();
            });

            // ========== إعدادات عناصر الطلب ==========
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(oi => oi.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(oi => oi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ========== إعدادات سلة التسوق ==========
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasOne(c => c.User)
                    .WithMany(u => u.CartItems)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(c => c.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(c => new { c.UserId, c.ProductId }).IsUnique();
            });

            // ========== إعدادات التقييمات ==========
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasOne(r => r.User)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Product)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(r => r.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(r => new { r.UserId, r.ProductId }).IsUnique();
            });

            // ========== إعدادات المدفوعات ==========
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasOne(p => p.Order)
                    .WithOne(o => o.Payment)
                    .HasForeignKey<Payment>(p => p.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(p => p.Status).HasConversion<string>();
                entity.Property(p => p.PaymentMethod).HasConversion<string>();
            });

            // ========== إعدادات سجلات الأدمن ==========
            modelBuilder.Entity<AdminLog>(entity =>
            {
                entity.HasOne(al => al.Admin)
                    .WithMany(u => u.AdminLogs)
                    .HasForeignKey(al => al.AdminId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ========== إعدادات المفضلة ==========
            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.HasOne(w => w.User)
                    .WithMany(u => u.WishlistItems)
                    .HasForeignKey(w => w.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(w => w.Product)
                    .WithMany(p => p.WishlistItems)
                    .HasForeignKey(w => w.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(w => new { w.UserId, w.ProductId }).IsUnique();
            });

            // ========== بيانات أولية - أدمن افتراضي ==========
            // كلمة المرور: Admin@123
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Name = "مدير النظام",
                Email = "admin@ecommerce.com",
                Password = "AdminPassword123Hashed",
                Phone = "0100000000",
                Role = Models.Enums.UserRole.Admin,
                IsActive = true,
                IsDeleted = false,
                Address = "القاهرة",
                City = "القاهرة",
                Country = "مصر",
                PostalCode = "11511",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            // ========== بيانات أولية - فئات افتراضية ==========
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "إلكترونيات",
                    Description = "أجهزة إلكترونية ومستلزماتها",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Category
                {
                    Id = 2,
                    Name = "ملابس",
                    Description = "ملابس رجالي ونسائي وأطفال",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Category
                {
                    Id = 3,
                    Name = "كتب",
                    Description = "كتب متنوعة في جميع المجالات",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }

        /// <summary>
        /// حفظ التغييرات مع تحديث تاريخ التعديل تلقائياً
        /// </summary>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}