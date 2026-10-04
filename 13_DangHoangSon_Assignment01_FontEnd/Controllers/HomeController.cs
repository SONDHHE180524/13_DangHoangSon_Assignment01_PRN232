using System.Diagnostics;
using _13_DangHoangSon_Assignment01_FontEnd.Models;
using _13_DangHoangSon_Assignment01_FontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace _13_DangHoangSon_Assignment01_FontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApiService _apiService;

        public HomeController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // Xem danh sách bài viết active cho cộng đồng / khách (không cần đăng nhập)
        public async Task<IActionResult> Index(string? search = null, short? categoryId = null)
        {
            var articles = await _apiService.GetNewsArticlesAsync(search, categoryId, activeOnly: true);
            var categories = await _apiService.GetCategoriesAsync();

            ViewBag.Categories = categories.Where(c => c.IsActive == true).ToList();
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentCategory = categoryId;

            return View(articles);
        }

        // Lấy chi tiết bài viết (JSON) để hiển thị trong popup modal đọc tin
        [HttpGet]
        public async Task<IActionResult> GetArticleDetails(string id)
        {
            var article = await _apiService.GetNewsArticleByIdAsync(id);
            if (article == null) return NotFound();
            return Json(article);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
