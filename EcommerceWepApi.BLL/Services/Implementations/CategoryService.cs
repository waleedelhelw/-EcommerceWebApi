using AutoMapper;
using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.DTOs.Category;
using EcommerceWepApi.BLL.Exceptions;
using EcommerceWepApi.BLL.Services.Interfaces;
using EcommerceWepApi.DAL.Models;
using EcommerceWepApi.DAL.Repositories.Interfaces;

namespace EcommerceWepApi.BLL.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAdminLogService _adminLogService;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IAdminLogService adminLogService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _adminLogService = adminLogService;
        }

        /// <summary>
        /// جلب جميع الفئات
        /// </summary>
        public async Task<ApiResponse<List<CategoryDto>>> GetAllCategoriesAsync(bool includeInactive = false)
        {
            var categories = await _unitOfWork.Categories.GetAllWithIncludeAsync(
                predicate: c => !c.IsDeleted && (includeInactive || c.IsActive),
                includes: c => c.Products);

            var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);
            return ApiResponse<List<CategoryDto>>.SuccessResponse(categoryDtos);
        }

        /// <summary>
        /// جلب فئة بالمعرّف
        /// </summary>
        public async Task<ApiResponse<CategoryDto>> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetFirstWithIncludeAsync(
                c => c.Id == id && !c.IsDeleted,
                c => c.Products);

            if (category == null)
            {
                throw new NotFoundException("الفئة", id);
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return ApiResponse<CategoryDto>.SuccessResponse(categoryDto);
        }

        /// <summary>
        /// إنشاء فئة جديدة
        /// </summary>
        public async Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto, int adminId)
        {
            // التحقق من عدم تكرار الاسم
            var exists = await _unitOfWork.Categories.AnyAsync(
                c => c.Name == dto.Name && !c.IsDeleted);
            if (exists)
            {
                return ApiResponse<CategoryDto>.FailureResponse("يوجد فئة بنفس الاسم بالفعل");
            }

            var category = _mapper.Map<Category>(dto);
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            // تسجيل العملية
            await _adminLogService.LogActionAsync(
                adminId, "إنشاء فئة", "Category", category.Id,
                newValues: $"Name: {category.Name}");

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return ApiResponse<CategoryDto>.SuccessResponse(categoryDto, "تم إنشاء الفئة بنجاح");
        }

        /// <summary>
        /// تحديث فئة
        /// </summary>
        public async Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto dto, int adminId)
        {
            var category = await _unitOfWork.Categories.FindAsync(c => c.Id == id && !c.IsDeleted);
            if (category == null)
            {
                throw new NotFoundException("الفئة", id);
            }

            // التحقق من عدم تكرار الاسم
            var exists = await _unitOfWork.Categories.AnyAsync(
                c => c.Name == dto.Name && c.Id != id && !c.IsDeleted);
            if (exists)
            {
                return ApiResponse<CategoryDto>.FailureResponse("يوجد فئة بنفس الاسم بالفعل");
            }

            var oldName = category.Name;
            category.Name = dto.Name;
            category.Description = dto.Description;
            category.ImageUrl = dto.ImageUrl;
            category.IsActive = dto.IsActive;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            // تسجيل العملية
            await _adminLogService.LogActionAsync(
                adminId, "تحديث فئة", "Category", id,
                oldValues: $"Name: {oldName}",
                newValues: $"Name: {category.Name}");

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return ApiResponse<CategoryDto>.SuccessResponse(categoryDto, "تم تحديث الفئة بنجاح");
        }

        /// <summary>
        /// حذف فئة - Soft Delete
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteCategoryAsync(int id, int adminId)
        {
            var category = await _unitOfWork.Categories.GetFirstWithIncludeAsync(
                c => c.Id == id && !c.IsDeleted,
                c => c.Products);

            if (category == null)
            {
                throw new NotFoundException("الفئة", id);
            }

            // التحقق من عدم وجود منتجات مرتبطة نشطة
            if (category.Products.Any(p => !p.IsDeleted))
            {
                return ApiResponse<bool>.FailureResponse("لا يمكن حذف فئة تحتوي على منتجات نشطة");
            }

            category.IsDeleted = true;
            category.IsActive = false;
            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            await _adminLogService.LogActionAsync(
                adminId, "حذف فئة", "Category", id,
                details: $"تم حذف الفئة: {category.Name}");

            return ApiResponse<bool>.SuccessResponse(true, "تم حذف الفئة بنجاح");
        }
    }
}