using AutoMapper;
using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.DTOs.User;
using EcommerceWepApi.BLL.Exceptions;
using EcommerceWepApi.BLL.Services.Interfaces;
using EcommerceWepApi.DAL.Models;
using EcommerceWepApi.DAL.Repositories.Interfaces;

namespace EcommerceWepApi.BLL.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAdminLogService _adminLogService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IAdminLogService adminLogService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _adminLogService = adminLogService;
        }

        /// <summary>
        /// جلب جميع المستخدمين مع ترقيم الصفحات
        /// </summary>
        public async Task<ApiResponse<PaginatedResponse<UserDto>>> GetAllUsersAsync(PaginationParams paginationParams)
        {
            var (users, totalCount) = await _unitOfWork.Users.GetPagedAsync(
                paginationParams.PageNumber,
                paginationParams.PageSize,
                predicate: u => !u.IsDeleted,
                orderBy: u => u.CreatedAt,
                isDescending: true);

            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

            var response = new PaginatedResponse<UserDto>
            {
                Items = userDtos,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize,
                TotalCount = totalCount
            };

            return ApiResponse<PaginatedResponse<UserDto>>.SuccessResponse(response);
        }

        /// <summary>
        /// جلب مستخدم بالمعرّف
        /// </summary>
        public async Task<ApiResponse<UserDto>> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.Users.FindAsync(u => u.Id == id && !u.IsDeleted);
            if (user == null)
            {
                throw new NotFoundException("المستخدم", id);
            }

            var userDto = _mapper.Map<UserDto>(user);
            return ApiResponse<UserDto>.SuccessResponse(userDto);
        }

        /// <summary>
        /// جلب الملف الشخصي
        /// </summary>
        public async Task<ApiResponse<UserDto>> GetProfileAsync(int userId)
        {
            return await GetUserByIdAsync(userId);
        }

        /// <summary>
        /// تحديث بيانات المستخدم
        /// </summary>
        public async Task<ApiResponse<UserDto>> UpdateUserAsync(int userId, UpdateUserDto dto)
        {
            var user = await _unitOfWork.Users.FindAsync(u => u.Id == userId && !u.IsDeleted);
            if (user == null)
            {
                throw new NotFoundException("المستخدم", userId);
            }

            // تحديث البيانات
            user.Name = dto.Name;
            user.Phone = dto.Phone;
            user.Address = dto.Address;
            user.City = dto.City;
            user.Country = dto.Country;
            user.PostalCode = dto.PostalCode;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            return ApiResponse<UserDto>.SuccessResponse(userDto, "تم تحديث البيانات بنجاح");
        }

        /// <summary>
        /// تفعيل / حظر مستخدم
        /// </summary>
        public async Task<ApiResponse<bool>> ToggleUserStatusAsync(int userId, int adminId)
        {
            var user = await _unitOfWork.Users.FindAsync(u => u.Id == userId && !u.IsDeleted);
            if (user == null)
            {
                throw new NotFoundException("المستخدم", userId);
            }

            var oldStatus = user.IsActive;
            user.IsActive = !user.IsActive;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // تسجيل العملية
            await _adminLogService.LogActionAsync(
                adminId,
                user.IsActive ? "تفعيل مستخدم" : "حظر مستخدم",
                "User",
                userId,
                oldStatus.ToString(),
                user.IsActive.ToString()
            );

            var message = user.IsActive ? "تم تفعيل المستخدم بنجاح" : "تم حظر المستخدم بنجاح";
            return ApiResponse<bool>.SuccessResponse(true, message);
        }

        /// <summary>
        /// حذف مستخدم - Soft Delete
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteUserAsync(int userId, int adminId)
        {
            var user = await _unitOfWork.Users.FindAsync(u => u.Id == userId && !u.IsDeleted);
            if (user == null)
            {
                throw new NotFoundException("المستخدم", userId);
            }

            user.IsDeleted = true;
            user.IsActive = false;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // تسجيل العملية
            await _adminLogService.LogActionAsync(
                adminId, "حذف مستخدم", "User", userId,
                details: $"تم حذف المستخدم: {user.Name} - {user.Email}"
            );

            return ApiResponse<bool>.SuccessResponse(true, "تم حذف المستخدم بنجاح");
        }
    }
}