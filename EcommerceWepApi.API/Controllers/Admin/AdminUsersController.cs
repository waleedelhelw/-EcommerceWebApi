using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.Services.Interfaces;

namespace EcommerceWepApi.API.Controllers.Admin
{
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminUsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// جلب جميع المستخدمين
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] PaginationParams paginationParams)
        {
            var result = await _userService.GetAllUsersAsync(paginationParams);
            return Ok(result);
        }

        /// <summary>
        /// جلب مستخدم بالمعرّف
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// تفعيل / حظر مستخدم
        /// </summary>
        [HttpPut("{id}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            var adminId = GetUserId();
            var result = await _userService.ToggleUserStatusAsync(id, adminId);
            return Ok(result);
        }

        /// <summary>
        /// حذف مستخدم
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var adminId = GetUserId();
            var result = await _userService.DeleteUserAsync(id, adminId);
            return Ok(result);
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}