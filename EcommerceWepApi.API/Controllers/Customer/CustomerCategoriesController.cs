using Microsoft.AspNetCore.Mvc;
using EcommerceWepApi.BLL.Services.Interfaces;

namespace EcommerceWepApi.API.Controllers.Customer
{
    [Route("api/customer/categories")]
    [ApiController]
    public class CustomerCategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CustomerCategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// جلب جميع الفئات النشطة
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync(includeInactive: false);
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
    }
}