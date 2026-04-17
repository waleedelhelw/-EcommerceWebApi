using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.Services.Interfaces;

namespace EcommerceWepApi.API.Controllers.Admin
{
    [Route("api/admin/logs")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminLogsController : ControllerBase
    {
        private readonly IAdminLogService _adminLogService;

        public AdminLogsController(IAdminLogService adminLogService)
        {
            _adminLogService = adminLogService;
        }

        /// <summary>
        /// جلب سجلات الأدمن
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetLogs([FromQuery] PaginationParams paginationParams)
        {
            var result = await _adminLogService.GetLogsAsync(paginationParams);
            return Ok(result);
        }
    }
}