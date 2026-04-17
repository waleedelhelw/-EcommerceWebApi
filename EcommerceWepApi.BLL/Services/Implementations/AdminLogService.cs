using AutoMapper;
using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.DTOs.AdminLog;
using EcommerceWepApi.BLL.Services.Interfaces;
using EcommerceWepApi.DAL.Models;
using EcommerceWepApi.DAL.Repositories.Interfaces;

namespace EcommerceWepApi.BLL.Services.Implementations
{
    public class AdminLogService : IAdminLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminLogService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// تسجيل عملية
        /// </summary>
        public async Task LogActionAsync(int adminId, string action, string? entityName = null,
            int? entityId = null, string? oldValues = null, string? newValues = null,
            string? details = null)
        {
            var log = new AdminLog
            {
                AdminId = adminId,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                OldValues = oldValues,
                NewValues = newValues,
                Details = details
            };

            await _unitOfWork.AdminLogs.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// جلب السجلات مع ترقيم الصفحات
        /// </summary>
        public async Task<ApiResponse<PaginatedResponse<AdminLogDto>>> GetLogsAsync(
            PaginationParams paginationParams)
        {
            var (logs, totalCount) = await _unitOfWork.AdminLogs.GetPagedAsync(
                paginationParams.PageNumber,
                paginationParams.PageSize,
                orderBy: l => l.CreatedAt,
                isDescending: true,
                includes: l => l.Admin);

            var logDtos = _mapper.Map<IEnumerable<AdminLogDto>>(logs);

            var response = new PaginatedResponse<AdminLogDto>
            {
                Items = logDtos,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize,
                TotalCount = totalCount
            };

            return ApiResponse<PaginatedResponse<AdminLogDto>>.SuccessResponse(response);
        }
    }
}