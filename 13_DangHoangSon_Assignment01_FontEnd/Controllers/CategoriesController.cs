using _13_DangHoangSon_Assignment01_FontEnd.Filters;
using _13_DangHoangSon_Assignment01_FontEnd.Models;
using _13_DangHoangSon_Assignment01_FontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace _13_DangHoangSon_Assignment01_FontEnd.Controllers
{
    [CustomAuthorize(1)] // Chỉ Role Staff (1) mới được quản lý danh mục
    public class CategoriesController : Controller
    {
        private readonly IApiService _apiService;

        public CategoriesController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // Danh sách danh mục + Tìm kiếm
        public async Task<IActionResult> Index(string? search = null)
        {
            var categories = await _apiService.GetCategoriesAsync(search);
            ViewBag.CurrentSearch = search;
            return View(categories);
        }

        // Lấy chi tiết danh mục để binding lên popup modal Sửa
        [HttpGet]
        public async Task<IActionResult> Get(short id)
        {
            var category = await _apiService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound("Không tìm thấy danh mục");
            return Json(category);
        }

        // Tạo mới danh mục qua popup dialog
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu danh mục không hợp lệ!";
                return RedirectToAction(nameof(Index));
            }

            var (success, message) = await _apiService.CreateCategoryAsync(model);
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

        // Cập nhật danh mục qua popup dialog
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu cập nhật danh mục không hợp lệ!";
                return RedirectToAction(nameof(Index));
            }

            var (success, message) = await _apiService.UpdateCategoryAsync(id, model);
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

        // Xóa danh mục kết hợp hộp thoại xác nhận
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(short id)
        {
            var (success, message) = await _apiService.DeleteCategoryAsync(id);
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
    }
}
