using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceWepApi.BLL.DTOs.User;
using EcommerceWepApi.BLL.Services.Interfaces;

namespace EcommerceWepApi.API.Controllers.Customer
{
    [Route("api/customer/profile")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// جلب بيانات الملف الشخصي
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();
            var result = await _userService.GetProfileAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// تحديث بيانات الملف الشخصي
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto dto)
        {
            var userId = GetUserId();
            var result = await _userService.UpdateUserAsync(userId, dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}