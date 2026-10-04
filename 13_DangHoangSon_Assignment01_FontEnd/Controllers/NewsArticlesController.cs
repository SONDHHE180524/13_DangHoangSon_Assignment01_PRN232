using _13_DangHoangSon_Assignment01_FontEnd.Filters;
using _13_DangHoangSon_Assignment01_FontEnd.Models;
using _13_DangHoangSon_Assignment01_FontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace _13_DangHoangSon_Assignment01_FontEnd.Controllers
{
    [CustomAuthorize(1)] // Chỉ Role Staff (1) mới được truy cập quản lý tin tức
    public class NewsArticlesController : Controller
    {
        private readonly IApiService _apiService;

        public NewsArticlesController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // Danh sách bài viết + Tìm kiếm
        public async Task<IActionResult> Index(string? search = null, short? categoryId = null)
        {
            var articles = await _apiService.GetNewsArticlesAsync(search, categoryId, activeOnly: false);
            var categories = await _apiService.GetCategoriesAsync();
            var tags = await _apiService.GetTagsAsync();

            ViewBag.Categories = categories;
            ViewBag.Tags = tags;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentCategory = categoryId;

            return View(articles);
        }

        // Lấy chi tiết bài viết dạng JSON để binding lên popup modal Sửa / Xem
        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            var article = await _apiService.GetNewsArticleByIdAsync(id);
            if (article == null) return NotFound("Không tìm thấy bài viết");

            var dto = new NewsArticleEditViewModel
            {
                NewsArticleId = article.NewsArticleId,
                NewsTitle = article.NewsTitle,
                Headline = article.Headline,
                NewsContent = article.NewsContent,
                NewsSource = article.NewsSource,
                CategoryId = article.CategoryId,
                NewsStatus = article.NewsStatus,
                CreatedById = article.CreatedById,
                UpdatedById = article.UpdatedById,
                TagIds = article.Tags.Select(t => t.TagId).ToList()
            };

            return Json(new { article, dto });
        }

        // Tạo bài viết mới qua popup dialog
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsArticleEditViewModel model)
        {
            var currentUserId = (short)(HttpContext.Session.GetInt32("UserId") ?? 0);
            model.CreatedById = currentUserId;
            model.UpdatedById = currentUserId;

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu nhập vào chưa hợp lệ, vui lòng kiểm tra lại!";
                return RedirectToAction(nameof(Index));
            }

            var (success, message) = await _apiService.CreateNewsArticleAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction(nameof(Index));
        }

        // Cập nhật bài viết qua popup dialog
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, NewsArticleEditViewModel model)
        {
            var currentUserId = (short)(HttpContext.Session.GetInt32("UserId") ?? 0);
            model.UpdatedById = currentUserId;

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu cập nhật chưa hợp lệ!";
                return RedirectToAction(nameof(Index));
            }

            var (success, message) = await _apiService.UpdateNewsArticleAsync(id, model);
            if (success)
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction(nameof(Index));
        }

        // Xóa bài viết với xác nhận
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var (success, message) = await _apiService.DeleteNewsArticleAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction(nameof(Index));
        }

        // Xem lịch sử bài viết được tạo bởi chính Staff này
        public async Task<IActionResult> History()
        {
            var currentUserId = (short)(HttpContext.Session.GetInt32("UserId") ?? 0);
            var allArticles = await _apiService.GetNewsArticlesAsync();
            var myArticles = allArticles.Where(a => a.CreatedById == currentUserId).ToList();

            return View(myArticles);
        }
    }
}
