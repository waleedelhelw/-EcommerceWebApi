using AutoMapper;
using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.DTOs.Cart;
using EcommerceWepApi.BLL.Exceptions;
using EcommerceWepApi.BLL.Services.Interfaces;
using EcommerceWepApi.DAL.Repositories.Interfaces;

namespace EcommerceWepApi.BLL.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// جلب سلة المستخدم
        /// </summary>
        public async Task<ApiResponse<CartSummaryDto>> GetCartAsync(int userId)
        {
            var cartItems = await _unitOfWork.Carts.GetAllWithIncludeAsync(
                c => c.UserId == userId,
                c => c.Product);

            var cartDtos = _mapper.Map<List<CartDto>>(cartItems);

            var summary = new CartSummaryDto
            {
                Items = cartDtos,
                TotalItems = cartDtos.Sum(c => c.Quantity),
                TotalPrice = cartDtos.Sum(c => c.TotalPrice)
            };

            return ApiResponse<CartSummaryDto>.SuccessResponse(summary);
        }

        /// <summary>
        /// إضافة منتج للسلة
        /// </summary>
        public async Task<ApiResponse<CartDto>> AddToCartAsync(int userId, AddToCartDto dto)
        {
            // التحقق من وجود المنتج
            var product = await _unitOfWork.Products.FindAsync(
                p => p.Id == dto.ProductId && !p.IsDeleted && p.IsActive);
            if (product == null)
            {
                return ApiResponse<CartDto>.FailureResponse("المنتج غير موجود أو غير متاح");
            }

            // التحقق من الكمية المتوفرة
            if (product.StockQuantity < dto.Quantity)
            {
                return ApiResponse<CartDto>.FailureResponse(
                    $"الكمية المطلوبة غير متوفرة. الكمية المتاحة: {product.StockQuantity}");
            }

            // التحقق من عدم وجود المنتج في السلة بالفعل
            var existingItem = await _unitOfWork.Carts.FindAsync(
                c => c.UserId == userId && c.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                // تحديث الكمية
                existingItem.Quantity += dto.Quantity;

                if (existingItem.Quantity > product.StockQuantity)
                {
                    return ApiResponse<CartDto>.FailureResponse(
                        $"الكمية الإجمالية تتجاوز المتوفر. الكمية المتاحة: {product.StockQuantity}");
                }

                _unitOfWork.Carts.Update(existingItem);
                await _unitOfWork.SaveChangesAsync();

                var updatedItem = await _unitOfWork.Carts.GetFirstWithIncludeAsync(
                    c => c.Id == existingItem.Id, c => c.Product);
                var updatedDto = _mapper.Map<CartDto>(updatedItem);
                return ApiResponse<CartDto>.SuccessResponse(updatedDto, "تم تحديث الكمية في السلة");
            }

            // إضافة عنصر جديد
            var cartItem = new DAL.Models.Cart
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };

            await _unitOfWork.Carts.AddAsync(cartItem);
            await _unitOfWork.SaveChangesAsync();

            var newItem = await _unitOfWork.Carts.GetFirstWithIncludeAsync(
                c => c.Id == cartItem.Id, c => c.Product);
            var cartDto = _mapper.Map<CartDto>(newItem);
            return ApiResponse<CartDto>.SuccessResponse(cartDto, "تم إضافة المنتج للسلة بنجاح");
        }

        /// <summary>
        /// تحديث كمية منتج في السلة
        /// </summary>
        public async Task<ApiResponse<CartDto>> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartDto dto)
        {
            var cartItem = await _unitOfWork.Carts.GetFirstWithIncludeAsync(
                c => c.Id == cartItemId && c.UserId == userId,
                c => c.Product);

            if (cartItem == null)
            {
                throw new NotFoundException("عنصر السلة", cartItemId);
            }

            // التحقق من الكمية المتوفرة
            if (cartItem.Product.StockQuantity < dto.Quantity)
            {
                return ApiResponse<CartDto>.FailureResponse(
                    $"الكمية المطلوبة غير متوفرة. الكمية المتاحة: {cartItem.Product.StockQuantity}");
            }

            cartItem.Quantity = dto.Quantity;
            _unitOfWork.Carts.Update(cartItem);
            await _unitOfWork.SaveChangesAsync();

            var cartDto = _mapper.Map<CartDto>(cartItem);
            return ApiResponse<CartDto>.SuccessResponse(cartDto, "تم تحديث الكمية بنجاح");
        }

        /// <summary>
        /// إزالة منتج من السلة
        /// </summary>
        public async Task<ApiResponse<bool>> RemoveFromCartAsync(int userId, int cartItemId)
        {
            var cartItem = await _unitOfWork.Carts.FindAsync(
                c => c.Id == cartItemId && c.UserId == userId);

            if (cartItem == null)
            {
                throw new NotFoundException("عنصر السلة", cartItemId);
            }

            _unitOfWork.Carts.Delete(cartItem);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "تم إزالة المنتج من السلة");
        }

        /// <summary>
        /// مسح السلة بالكامل
        /// </summary>
        public async Task<ApiResponse<bool>> ClearCartAsync(int userId)
        {
            var cartItems = await _unitOfWork.Carts.GetAllAsync(c => c.UserId == userId);
            _unitOfWork.Carts.DeleteRange(cartItems);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "تم مسح السلة بالكامل");
        }
    }
}