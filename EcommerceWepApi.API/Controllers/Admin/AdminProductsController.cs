using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceWepApi.BLL.DTOs.Product;
using EcommerceWepApi.BLL.Services.Interfaces;

namespace EcommerceWepApi.API.Controllers.Admin
{
    [Route("api/admin/products")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public AdminProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// جلب جميع المنتجات مع الفلاتر
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductFilterDto filter)
        {
            var result = await _productService.GetAllProductsAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// جلب منتج بالمعرّف
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// إنشاء منتج جديد
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            var adminId = GetUserId();
            var result = await _productService.CreateProductAsync(dto, adminId);
            if (!result.Success)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetProductById), new { id = result.Data!.Id }, result);
        }

        /// <summary>
        /// تحديث منتج
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
        {
            var adminId = GetUserId();
            var result = await _productService.UpdateProductAsync(id, dto, adminId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// حذف منتج
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var adminId = GetUserId();
            var result = await _productService.DeleteProductAsync(id, adminId);
            return Ok(result);
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}