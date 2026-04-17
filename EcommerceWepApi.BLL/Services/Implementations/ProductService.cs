using AutoMapper;
using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.DTOs.Product;
using EcommerceWepApi.BLL.Exceptions;
using EcommerceWepApi.BLL.Services.Interfaces;
using EcommerceWepApi.DAL.Models;
using EcommerceWepApi.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace EcommerceWepApi.BLL.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAdminLogService _adminLogService;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IAdminLogService adminLogService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _adminLogService = adminLogService;
        }

        /// <summary>
        /// جلب المنتجات مع فلاتر وترقيم صفحات
        /// </summary>
        public async Task<ApiResponse<PaginatedResponse<ProductDto>>> GetAllProductsAsync(ProductFilterDto filter)
        {
            // بناء شرط التصفية
            Expression<Func<Product, bool>> predicate = p => !p.IsDeleted && p.IsActive;

            // تصفية حسب الفئة
            if (filter.CategoryId.HasValue)
            {
                var categoryId = filter.CategoryId.Value;
                predicate = p => !p.IsDeleted && p.IsActive && p.CategoryId == categoryId;
            }

            // تصفية حسب السعر
            if (filter.MinPrice.HasValue && filter.MaxPrice.HasValue)
            {
                var minPrice = filter.MinPrice.Value;
                var maxPrice = filter.MaxPrice.Value;
                var catId = filter.CategoryId;

                predicate = p => !p.IsDeleted && p.IsActive
                    && p.Price >= minPrice && p.Price <= maxPrice
                    && (!catId.HasValue || p.CategoryId == catId.Value);
            }

            // تصفية حسب التقييم
            if (filter.MinRating.HasValue)
            {
                var minRating = filter.MinRating.Value;
                var catId = filter.CategoryId;
                var minP = filter.MinPrice;
                var maxP = filter.MaxPrice;

                predicate = p => !p.IsDeleted && p.IsActive
                    && p.Rating >= minRating
                    && (!catId.HasValue || p.CategoryId == catId.Value)
                    && (!minP.HasValue || p.Price >= minP.Value)
                    && (!maxP.HasValue || p.Price <= maxP.Value);
            }

            // البحث بالاسم
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                var catId = filter.CategoryId;
                var minP = filter.MinPrice;
                var maxP = filter.MaxPrice;
                var minR = filter.MinRating;

                predicate = p => !p.IsDeleted && p.IsActive
                    && (p.Name.ToLower().Contains(searchTerm)
                        || (p.Description != null && p.Description.ToLower().Contains(searchTerm)))
                    && (!catId.HasValue || p.CategoryId == catId.Value)
                    && (!minP.HasValue || p.Price >= minP.Value)
                    && (!maxP.HasValue || p.Price <= maxP.Value)
                    && (!minR.HasValue || p.Rating >= minR.Value);
            }

            // تحديد الترتيب
            Expression<Func<Product, object>> orderBy = filter.SortBy?.ToLower() switch
            {
                "price" => p => p.Price,
                "rating" => p => p.Rating,
                "name" => p => p.Name,
                _ => p => p.CreatedAt // الافتراضي: الأحدث
            };

            bool isDescending = filter.SortBy?.ToLower() switch
            {
                "price" => filter.IsDescending,
                "rating" => true,
                "name" => false,
                _ => true
            };

            var (products, totalCount) = await _unitOfWork.Products.GetPagedAsync(
                filter.PageNumber,
                filter.PageSize,
                predicate,
                orderBy,
                isDescending,
                p => p.Category);

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            var response = new PaginatedResponse<ProductDto>
            {
                Items = productDtos,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalCount = totalCount
            };

            return ApiResponse<PaginatedResponse<ProductDto>>.SuccessResponse(response);
        }

        /// <summary>
        /// جلب منتج بالمعرّف
        /// </summary>
        public async Task<ApiResponse<ProductDto>> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetFirstWithIncludeAsync(
                p => p.Id == id && !p.IsDeleted,
                p => p.Category);

            if (product == null)
            {
                throw new NotFoundException("المنتج", id);
            }

            var productDto = _mapper.Map<ProductDto>(product);
            return ApiResponse<ProductDto>.SuccessResponse(productDto);
        }

        /// <summary>
        /// جلب المنتجات حسب الفئة
        /// </summary>
        public async Task<ApiResponse<List<ProductDto>>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _unitOfWork.Products.GetAllWithIncludeAsync(
                p => p.CategoryId == categoryId && !p.IsDeleted && p.IsActive,
                p => p.Category);

            var productDtos = _mapper.Map<List<ProductDto>>(products);
            return ApiResponse<List<ProductDto>>.SuccessResponse(productDtos);
        }

        /// <summary>
        /// جلب المنتجات المميزة
        /// </summary>
        public async Task<ApiResponse<List<ProductDto>>> GetFeaturedProductsAsync(int count = 10)
        {
            var (products, _) = await _unitOfWork.Products.GetPagedAsync(
                1, count,
                p => !p.IsDeleted && p.IsActive,
                p => p.Rating,
                true,
                p => p.Category);

            var productDtos = _mapper.Map<List<ProductDto>>(products);
            return ApiResponse<List<ProductDto>>.SuccessResponse(productDtos);
        }

        /// <summary>
        /// جلب أحدث المنتجات
        /// </summary>
        public async Task<ApiResponse<List<ProductDto>>> GetNewProductsAsync(int count = 10)
        {
            var (products, _) = await _unitOfWork.Products.GetPagedAsync(
                1, count,
                p => !p.IsDeleted && p.IsActive,
                p => p.CreatedAt,
                true,
                p => p.Category);

            var productDtos = _mapper.Map<List<ProductDto>>(products);
            return ApiResponse<List<ProductDto>>.SuccessResponse(productDtos);
        }

        /// <summary>
        /// جلب المنتجات ذات الصلة
        /// </summary>
        public async Task<ApiResponse<List<ProductDto>>> GetRelatedProductsAsync(int productId, int count = 5)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new NotFoundException("المنتج", productId);
            }

            var (relatedProducts, _) = await _unitOfWork.Products.GetPagedAsync(
                1, count,
                p => p.CategoryId == product.CategoryId && p.Id != productId && !p.IsDeleted && p.IsActive,
                p => p.Rating,
                true,
                p => p.Category);

            var productDtos = _mapper.Map<List<ProductDto>>(relatedProducts);
            return ApiResponse<List<ProductDto>>.SuccessResponse(productDtos);
        }

        /// <summary>
        /// إنشاء منتج جديد
        /// </summary>
        public async Task<ApiResponse<ProductDto>> CreateProductAsync(CreateProductDto dto, int adminId)
        {
            // التحقق من وجود الفئة
            var categoryExists = await _unitOfWork.Categories.AnyAsync(
                c => c.Id == dto.CategoryId && !c.IsDeleted);
            if (!categoryExists)
            {
                return ApiResponse<ProductDto>.FailureResponse("الفئة المحددة غير موجودة");
            }

            var product = _mapper.Map<Product>(dto);
            product.CreatedById = adminId;

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            // جلب المنتج مع الفئة
            var createdProduct = await _unitOfWork.Products.GetFirstWithIncludeAsync(
                p => p.Id == product.Id, p => p.Category);

            await _adminLogService.LogActionAsync(
                adminId, "إنشاء منتج", "Product", product.Id,
                newValues: $"Name: {product.Name}, Price: {product.Price}");

            var productDto = _mapper.Map<ProductDto>(createdProduct);
            return ApiResponse<ProductDto>.SuccessResponse(productDto, "تم إنشاء المنتج بنجاح");
        }

        /// <summary>
        /// تحديث منتج
        /// </summary>
        public async Task<ApiResponse<ProductDto>> UpdateProductAsync(int id, UpdateProductDto dto, int adminId)
        {
            var product = await _unitOfWork.Products.FindAsync(p => p.Id == id && !p.IsDeleted);
            if (product == null)
            {
                throw new NotFoundException("المنتج", id);
            }

            var categoryExists = await _unitOfWork.Categories.AnyAsync(
                c => c.Id == dto.CategoryId && !c.IsDeleted);
            if (!categoryExists)
            {
                return ApiResponse<ProductDto>.FailureResponse("الفئة المحددة غير موجودة");
            }

            var oldValues = $"Name: {product.Name}, Price: {product.Price}";

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.CategoryId = dto.CategoryId;
            product.ImageUrl = dto.ImageUrl;
            product.IsActive = dto.IsActive;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            var updatedProduct = await _unitOfWork.Products.GetFirstWithIncludeAsync(
                p => p.Id == id, p => p.Category);

            await _adminLogService.LogActionAsync(
                adminId, "تحديث منتج", "Product", id,
                oldValues: oldValues,
                newValues: $"Name: {product.Name}, Price: {product.Price}");

            var productDto = _mapper.Map<ProductDto>(updatedProduct);
            return ApiResponse<ProductDto>.SuccessResponse(productDto, "تم تحديث المنتج بنجاح");
        }

        /// <summary>
        /// حذف منتج - Soft Delete
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteProductAsync(int id, int adminId)
        {
            var product = await _unitOfWork.Products.FindAsync(p => p.Id == id && !p.IsDeleted);
            if (product == null)
            {
                throw new NotFoundException("المنتج", id);
            }

            product.IsDeleted = true;
            product.IsActive = false;
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            await _adminLogService.LogActionAsync(
                adminId, "حذف منتج", "Product", id,
                details: $"تم حذف المنتج: {product.Name}");

            return ApiResponse<bool>.SuccessResponse(true, "تم حذف المنتج بنجاح");
        }
    }
}