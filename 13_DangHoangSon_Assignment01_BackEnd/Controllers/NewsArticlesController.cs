using _13_DangHoangSon_Assignment01_BackEnd.DTOs;
using _13_DangHoangSon_Assignment01_BackEnd.Models;
using _13_DangHoangSon_Assignment01_BackEnd.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace _13_DangHoangSon_Assignment01_BackEnd.Controllers
{
    [Route("api/NewsArticles")]
    [ApiController]
    public class NewsArticlesController : ControllerBase
    {
        private readonly INewsArticleRepository _newsRepo;

        public NewsArticlesController(INewsArticleRepository newsRepo)
        {
            _newsRepo = newsRepo;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var articles = await _newsRepo.GetAllNewsArticlesAsync();
            return Ok(articles.AsQueryable());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return await Get();
            var article = await _newsRepo.GetNewsArticleByIdAsync(id);
            if (article == null) return NotFound("Không tìm thấy bài viết.");
            return Ok(article);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NewsArticleRequestDto request)
        {
            if (request == null || !ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _newsRepo.GetNewsArticleByIdAsync(request.NewsArticleId);
            if (existing != null) return BadRequest("Mã bài viết đã tồn tại.");

            var article = new NewsArticle
            {
                NewsArticleId = request.NewsArticleId,
                NewsTitle = request.NewsTitle,
                Headline = request.Headline,
                CreatedDate = DateTime.Now,
                NewsContent = request.NewsContent,
                NewsSource = request.NewsSource,
                CategoryId = request.CategoryId,
                NewsStatus = request.NewsStatus ?? true,
                CreatedById = request.CreatedById,
                UpdatedById = request.CreatedById,
                ModifiedDate = DateTime.Now
            };

            await _newsRepo.AddNewsArticleAsync(article, request.TagIds);
            return Ok(article);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] NewsArticleRequestDto request)
        {
            if (request == null) return BadRequest("Dữ liệu không hợp lệ.");
            if (id != request.NewsArticleId && !string.IsNullOrEmpty(id)) request.NewsArticleId = id;

            var existing = await _newsRepo.GetNewsArticleByIdAsync(id);
            if (existing == null) return NotFound("Không tìm thấy bài viết.");

            var article = new NewsArticle
            {
                NewsArticleId = request.NewsArticleId,
                NewsTitle = request.NewsTitle,
                Headline = request.Headline,
                CreatedDate = existing.CreatedDate, // Giữ nguyên ngày tạo ban đầu
                NewsContent = request.NewsContent,
                NewsSource = request.NewsSource,
                CategoryId = request.CategoryId,
                NewsStatus = request.NewsStatus,
                CreatedById = existing.CreatedById,
                UpdatedById = request.UpdatedById,
                ModifiedDate = DateTime.Now
            };

            await _newsRepo.UpdateNewsArticleAsync(article, request.TagIds);
            return Ok(article);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _newsRepo.DeleteNewsArticleAsync(id);
            if (!deleted) return NotFound("Không tìm thấy bài viết để xóa.");
            return NoContent();
        }

        // Báo cáo thống kê theo khoảng thời gian (StartDate -> EndDate) cho Admin
        [HttpGet("/api/NewsArticles/report")]
        public async Task<IActionResult> GetReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var allArticles = await _newsRepo.GetAllNewsArticlesAsync();
            var result = allArticles
                .Where(a => a.CreatedDate.HasValue
                            && a.CreatedDate.Value.Date >= startDate.Date
                            && a.CreatedDate.Value.Date <= endDate.Date)
                .OrderByDescending(a => a.CreatedDate)
                .ToList();

            return Ok(result);
        }
    }
}