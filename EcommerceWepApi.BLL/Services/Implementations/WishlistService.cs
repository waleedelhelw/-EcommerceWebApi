using AutoMapper;
using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.DTOs.Wishlist;
using EcommerceWepApi.BLL.Exceptions;
using EcommerceWepApi.BLL.Services.Interfaces;
using EcommerceWepApi.DAL.Models;
using EcommerceWepApi.DAL.Repositories.Interfaces;

namespace EcommerceWepApi.BLL.Services.Implementations
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WishlistService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// جلب المفضلة
        /// </summary>
        public async Task<ApiResponse<List<WishlistDto>>> GetWishlistAsync(int userId)
        {
            var wishlistItems = await _unitOfWork.Wishlists.GetAllWithIncludeAsync(
                w => w.UserId == userId,
                w => w.Product);

            var wishlistDtos = _mapper.Map<List<WishlistDto>>(wishlistItems);
            return ApiResponse<List<WishlistDto>>.SuccessResponse(wishlistDtos);
        }

        /// <summary>
        /// إضافة منتج للمفضلة
        /// </summary>
        public async Task<ApiResponse<WishlistDto>> AddToWishlistAsync(int userId, int productId)
        {
            // التحقق من وجود المنتج
            var product = await _unitOfWork.Products.FindAsync(
                p => p.Id == productId && !p.IsDeleted && p.IsActive);
            if (product == null)
            {
                return ApiResponse<WishlistDto>.FailureResponse("المنتج غير موجود أو غير متاح");
            }

            // التحقق من عدم وجود المنتج في المفضلة
            var exists = await _unitOfWork.Wishlists.AnyAsync(
                w => w.UserId == userId && w.ProductId == productId);
            if (exists)
            {
                return ApiResponse<WishlistDto>.FailureResponse("المنتج موجود في المفضلة بالفعل");
            }

            var wishlistItem = new Wishlist
            {
                UserId = userId,
                ProductId = productId
            };

            await _unitOfWork.Wishlists.AddAsync(wishlistItem);
            await _unitOfWork.SaveChangesAsync();

            var newItem = await _unitOfWork.Wishlists.GetFirstWithIncludeAsync(
                w => w.Id == wishlistItem.Id, w => w.Product);

            var wishlistDto = _mapper.Map<WishlistDto>(newItem);
            return ApiResponse<WishlistDto>.SuccessResponse(wishlistDto, "تم إضافة المنتج للمفضلة بنجاح");
        }

        /// <summary>
        /// إزالة منتج من المفضلة
        /// </summary>
        public async Task<ApiResponse<bool>> RemoveFromWishlistAsync(int userId, int wishlistId)
        {
            var wishlistItem = await _unitOfWork.Wishlists.FindAsync(
                w => w.Id == wishlistId && w.UserId == userId);

            if (wishlistItem == null)
            {
                throw new NotFoundException("عنصر المفضلة", wishlistId);
            }

            _unitOfWork.Wishlists.Delete(wishlistItem);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "تم إزالة المنتج من المفضلة");
        }
    }
}