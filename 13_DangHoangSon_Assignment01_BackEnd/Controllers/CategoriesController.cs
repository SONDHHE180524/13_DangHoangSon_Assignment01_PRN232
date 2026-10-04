using _13_DangHoangSon_Assignment01_BackEnd.DTOs;
using _13_DangHoangSon_Assignment01_BackEnd.Models;
using _13_DangHoangSon_Assignment01_BackEnd.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace _13_DangHoangSon_Assignment01_BackEnd.Controllers
{
    [Route("api/Categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoriesController(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var categories = await _categoryRepo.GetAllCategoriesAsync();
            return Ok(categories.AsQueryable());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(short id)
        {
            var category = await _categoryRepo.GetCategoryByIdAsync(id);
            if (category == null) return NotFound("Không tìm thấy danh mục.");
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CategoryRequestDto request)
        {
            if (request == null || !ModelState.IsValid) return BadRequest("Dữ liệu danh mục không hợp lệ.");

            var category = new Category
            {
                CategoryName = request.CategoryName,
                CategoryDesciption = request.CategoryDesciption,
                ParentCategoryId = request.ParentCategoryId,
                IsActive = request.IsActive ?? true
            };

            await _categoryRepo.AddCategoryAsync(category);
            return Ok(category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(short id, [FromBody] CategoryRequestDto request)
        {
            if (request == null) return BadRequest("Dữ liệu danh mục không hợp lệ.");
            if (id != request.CategoryId && request.CategoryId != 0) request.CategoryId = id;

            var existing = await _categoryRepo.GetCategoryByIdAsync(id);
            if (existing == null) return NotFound("Không tìm thấy danh mục.");

            var category = new Category
            {
                CategoryId = id,
                CategoryName = request.CategoryName,
                CategoryDesciption = request.CategoryDesciption,
                ParentCategoryId = request.ParentCategoryId,
                IsActive = request.IsActive
            };

            await _categoryRepo.UpdateCategoryAsync(category);
            return Ok(category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(short id)
        {
            var deleted = await _categoryRepo.DeleteCategoryAsync(id);
            if (!deleted)
            {
                return BadRequest("Không thể xóa danh mục này vì đã có bài viết trực thuộc!");
            }
            return NoContent();
        }
    }
}