using AutoMapper;
using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.DTOs.Review;
using EcommerceWepApi.BLL.Exceptions;
using EcommerceWepApi.BLL.Services.Interfaces;
using EcommerceWepApi.DAL.Models;
using EcommerceWepApi.DAL.Models.Enums;
using EcommerceWepApi.DAL.Repositories.Interfaces;

namespace EcommerceWepApi.BLL.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAdminLogService _adminLogService;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper, IAdminLogService adminLogService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _adminLogService = adminLogService;
        }

        /// <summary>
        /// جلب تقييمات منتج معين
        /// </summary>
        public async Task<ApiResponse<List<ReviewDto>>> GetProductReviewsAsync(int productId)
        {
            var reviews = await _unitOfWork.Reviews.GetAllWithIncludeAsync(
                r => r.ProductId == productId && !r.IsDeleted && r.IsApproved,
                r => r.User,
                r => r.Product);

            var reviewDtos = _mapper.Map<List<ReviewDto>>(reviews);
            return ApiResponse<List<ReviewDto>>.SuccessResponse(reviewDtos);
        }

        /// <summary>
        /// جلب جميع التقييمات (أدمن)
        /// </summary>
        public async Task<ApiResponse<PaginatedResponse<ReviewDto>>> GetAllReviewsAsync(
            PaginationParams paginationParams)
        {
            var (reviews, totalCount) = await _unitOfWork.Reviews.GetPagedAsync(
                paginationParams.PageNumber,
                paginationParams.PageSize,
                predicate: r => !r.IsDeleted,
                orderBy: r => r.CreatedAt,
                isDescending: true,
                r => r.User,
                r => r.Product);

            var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);

            var response = new PaginatedResponse<ReviewDto>
            {
                Items = reviewDtos,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize,
                TotalCount = totalCount
            };

            return ApiResponse<PaginatedResponse<ReviewDto>>.SuccessResponse(response);
        }

        /// <summary>
        /// إضافة تقييم (بعد الشراء فقط)
        /// </summary>
        public async Task<ApiResponse<ReviewDto>> CreateReviewAsync(int userId, CreateReviewDto dto)
        {
            // التحقق من وجود المنتج
            var product = await _unitOfWork.Products.FindAsync(
                p => p.Id == dto.ProductId && !p.IsDeleted);
            if (product == null)
            {
                throw new NotFoundException("المنتج", dto.ProductId);
            }

            // التحقق من أن المستخدم اشترى المنتج
            var hasPurchased = await _unitOfWork.OrderItems.AnyAsync(
                oi => oi.Product.Id == dto.ProductId
                    && oi.Order.UserId == userId
                    && oi.Order.Status == OrderStatus.Delivered);

            if (!hasPurchased)
            {
                return ApiResponse<ReviewDto>.FailureResponse(
                    "لا يمكنك تقييم منتج لم تشتريه أو لم يتم تسليمه بعد");
            }

            // التحقق من عدم وجود تقييم سابق
            var existingReview = await _unitOfWork.Reviews.FindAsync(
                r => r.UserId == userId && r.ProductId == dto.ProductId && !r.IsDeleted);

            if (existingReview != null)
            {
                return ApiResponse<ReviewDto>.FailureResponse("لقد قمت بتقييم هذا المنتج بالفعل");
            }

            // إنشاء التقييم
            var review = new Review
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                Title = dto.Title,
                IsApproved = false,
                IsDeleted = false
            };

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            // تحديث تقييم المنتج
            await UpdateProductRatingAsync(dto.ProductId);

            var createdReview = await _unitOfWork.Reviews.GetFirstWithIncludeAsync(
                r => r.Id == review.Id,
                r => r.User,
                r => r.Product);

            var reviewDto = _mapper.Map<ReviewDto>(createdReview);
            return ApiResponse<ReviewDto>.SuccessResponse(reviewDto,
                "تم إضافة التقييم بنجاح وسيتم مراجعته من قبل الإدارة");
        }

        /// <summary>
        /// تعديل تقييم
        /// </summary>
        public async Task<ApiResponse<ReviewDto>> UpdateReviewAsync(int userId, int reviewId, UpdateReviewDto dto)
        {
            var review = await _unitOfWork.Reviews.GetFirstWithIncludeAsync(
                r => r.Id == reviewId && r.UserId == userId && !r.IsDeleted,
                r => r.User,
                r => r.Product);

            if (review == null)
            {
                throw new NotFoundException("التقييم", reviewId);
            }

            review.Rating = dto.Rating;
            review.Comment = dto.Comment;
            review.Title = dto.Title;
            review.IsApproved = false; // يحتاج موافقة مرة ثانية بعد التعديل

            _unitOfWork.Reviews.Update(review);
            await _unitOfWork.SaveChangesAsync();

            // تحديث تقييم المنتج
            await UpdateProductRatingAsync(review.ProductId);

            var reviewDto = _mapper.Map<ReviewDto>(review);
            return ApiResponse<ReviewDto>.SuccessResponse(reviewDto, "تم تعديل التقييم بنجاح");
        }

        /// <summary>
        /// حذف تقييم
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteReviewAsync(int userId, int reviewId, bool isAdmin = false)
        {
            Review? review;

            if (isAdmin)
            {
                review = await _unitOfWork.Reviews.FindAsync(r => r.Id == reviewId && !r.IsDeleted);
            }
            else
            {
                review = await _unitOfWork.Reviews.FindAsync(
                    r => r.Id == reviewId && r.UserId == userId && !r.IsDeleted);
            }

            if (review == null)
            {
                throw new NotFoundException("التقييم", reviewId);
            }

            review.IsDeleted = true;
            _unitOfWork.Reviews.Update(review);
            await _unitOfWork.SaveChangesAsync();

            // تحديث تقييم المنتج
            await UpdateProductRatingAsync(review.ProductId);

            return ApiResponse<bool>.SuccessResponse(true, "تم حذف التقييم بنجاح");
        }

        /// <summary>
        /// الموافقة على تقييم (أدمن)
        /// </summary>
        public async Task<ApiResponse<bool>> ApproveReviewAsync(int reviewId, int adminId)
        {
            var review = await _unitOfWork.Reviews.FindAsync(r => r.Id == reviewId && !r.IsDeleted);
            if (review == null)
            {
                throw new NotFoundException("التقييم", reviewId);
            }

            review.IsApproved = true;
            _unitOfWork.Reviews.Update(review);
            await _unitOfWork.SaveChangesAsync();

            // تسجيل العملية
            await _adminLogService.LogActionAsync(
                adminId, "الموافقة على تقييم", "Review", reviewId);

            return ApiResponse<bool>.SuccessResponse(true, "تمت الموافقة على التقييم بنجاح");
        }

        /// <summary>
        /// تحديث متوسط تقييم المنتج
        /// </summary>
        private async Task UpdateProductRatingAsync(int productId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null) return;

            var reviews = await _unitOfWork.Reviews.GetAllAsync(
                r => r.ProductId == productId && !r.IsDeleted && r.IsApproved);

            var reviewList = reviews.ToList();
            if (reviewList.Any())
            {
                product.Rating = (decimal)reviewList.Average(r => r.Rating);
                product.TotalRatings = reviewList.Count;
            }
            else
            {
                product.Rating = 0;
                product.TotalRatings = 0;
            }

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}