using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceWepApi.BLL.DTOs.Category;
using EcommerceWepApi.BLL.Services.Interfaces;

namespace EcommerceWepApi.API.Controllers.Admin
{
    [Route("api/admin/categories")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminCategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public AdminCategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// جلب جميع الفئات (مع الغير نشطة)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync(includeInactive: true);
            return Ok(result);
        }

        /// <summary>
        /// جلب فئة بالمعرّف
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// إنشاء فئة جديدة
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            var adminId = GetUserId();
            var result = await _categoryService.CreateCategoryAsync(dto, adminId);
            if (!result.Success)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetCategoryById), new { id = result.Data!.Id }, result);
        }

        /// <summary>
        /// تحديث فئة
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto dto)
        {
            var adminId = GetUserId();
            var result = await _categoryService.UpdateCategoryAsync(id, dto, adminId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// حذف فئة
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var adminId = GetUserId();
            var result = await _categoryService.DeleteCategoryAsync(id, adminId);
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