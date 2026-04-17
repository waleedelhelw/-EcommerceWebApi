using AutoMapper;
using EcommerceWepApi.DAL.Models;
using EcommerceWepApi.BLL.DTOs.User;
using EcommerceWepApi.BLL.DTOs.Category;
using EcommerceWepApi.BLL.DTOs.Product;
using EcommerceWepApi.BLL.DTOs.Cart;
using EcommerceWepApi.BLL.DTOs.Order;
using EcommerceWepApi.BLL.DTOs.Review;
using EcommerceWepApi.BLL.DTOs.Payment;
using EcommerceWepApi.BLL.DTOs.Wishlist;
using EcommerceWepApi.BLL.DTOs.AdminLog;
using EcommerceWepApi.BLL.DTOs.Dashboard;

namespace EcommerceWepApi.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ========== User Mappings ==========
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
            CreateMap<UpdateUserDto, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ========== Category Mappings ==========
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ProductCount,
                    opt => opt.MapFrom(src => src.Products.Count(p => !p.IsDeleted)));
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ========== Product Mappings ==========
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ========== Cart Mappings ==========
            CreateMap<Cart, CartDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price));

            // ========== Order Mappings ==========
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl));

            CreateMap<Order, RecentOrderDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // ========== Review Mappings ==========
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            // ========== Payment Mappings ==========
            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()));

            // ========== Wishlist Mappings ==========
            CreateMap<Wishlist, WishlistDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.IsInStock, opt => opt.MapFrom(src => src.Product.StockQuantity > 0));

            // ========== AdminLog Mappings ==========
            CreateMap<AdminLog, AdminLogDto>()
                .ForMember(dest => dest.AdminName, opt => opt.MapFrom(src => src.Admin.Name));
        }
    }
}