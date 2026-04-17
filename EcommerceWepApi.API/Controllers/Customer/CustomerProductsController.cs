using Microsoft.AspNetCore.Mvc;
using EcommerceWepApi.BLL.DTOs.Product;
using EcommerceWepApi.BLL.Services.Interfaces;

namespace EcommerceWepApi.API.Controllers.Customer
{
    [Route("api/customer/products")]
    [ApiController]
    public class CustomerProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public CustomerProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// جلب جميع المنتجات مع الفلاتر والبحث والترقيم
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
        /// جلب المنتجات حسب الفئة
        /// </summary>
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var result = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(result);
        }

        /// <summary>
        /// جلب المنتجات المميزة (الأعلى تقييماً)
        /// </summary>
        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedProducts([FromQuery] int count = 10)
        {
            var result = await _productService.GetFeaturedProductsAsync(count);
            return Ok(result);
        }

        /// <summary>
        /// جلب أحدث المنتجات
        /// </summary>
        [HttpGet("new")]
        public async Task<IActionResult> GetNewProducts([FromQuery] int count = 10)
        {
            var result = await _productService.GetNewProductsAsync(count);
            return Ok(result);
        }

        /// <summary>
        /// جلب المنتجات ذات الصلة
        /// </summary>
        [HttpGet("{id}/related")]
        public async Task<IActionResult> GetRelatedProducts(int id, [FromQuery] int count = 5)
        {
            var result = await _productService.GetRelatedProductsAsync(id, count);
            return Ok(result);
        }
    }
}