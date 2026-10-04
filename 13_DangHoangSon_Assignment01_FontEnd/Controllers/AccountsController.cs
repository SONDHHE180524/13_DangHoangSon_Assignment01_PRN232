using _13_DangHoangSon_Assignment01_FontEnd.Filters;
using _13_DangHoangSon_Assignment01_FontEnd.Models;
using _13_DangHoangSon_Assignment01_FontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace _13_DangHoangSon_Assignment01_FontEnd.Controllers
{
    [CustomAuthorize(0)] // Chỉ Role Admin (0) mới được quản lý tài khoản
    public class AccountsController : Controller
    {
        private readonly IApiService _apiService;

        public AccountsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // Danh sách tài khoản + Tìm kiếm
        public async Task<IActionResult> Index(string? search = null)
        {
            var accounts = await _apiService.GetAccountsAsync(search);
            ViewBag.CurrentSearch = search;
            return View(accounts);
        }

        // Lấy chi tiết tài khoản để binding lên popup modal Sửa
        [HttpGet]
        public async Task<IActionResult> Get(short id)
        {
            var account = await _apiService.GetAccountByIdAsync(id);
            if (account == null) return NotFound("Không tìm thấy tài khoản");
            return Json(account);
        }

        // Tạo tài khoản mới qua popup dialog
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SystemAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu tài khoản không hợp lệ!";
                return RedirectToAction(nameof(Index));
            }

            var (success, message) = await _apiService.CreateAccountAsync(model);
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

        // Cập nhật tài khoản qua popup dialog
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, SystemAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu cập nhật tài khoản không hợp lệ!";
                return RedirectToAction(nameof(Index));
            }

            var (success, message) = await _apiService.UpdateAccountAsync(id, model);
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

        // Xóa tài khoản kết hợp hộp thoại xác nhận
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(short id)
        {
            var (success, message) = await _apiService.DeleteAccountAsync(id);
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
