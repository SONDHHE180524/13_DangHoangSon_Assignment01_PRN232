using _13_DangHoangSon_Assignment01_FontEnd.Filters;
using _13_DangHoangSon_Assignment01_FontEnd.Models;
using _13_DangHoangSon_Assignment01_FontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace _13_DangHoangSon_Assignment01_FontEnd.Controllers
{
    [CustomAuthorize(1, 2)] // Staff hoặc Lecturer quản lý hồ sơ của mình
    public class ProfileController : Controller
    {
        private readonly IApiService _apiService;

        public ProfileController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = (short)(HttpContext.Session.GetInt32("UserId") ?? 0);
            var account = await _apiService.GetAccountByIdAsync(userId);
            if (account == null)
            {
                return NotFound("Không tìm thấy thông tin tài khoản!");
            }

            var model = new UserProfileViewModel
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountPassword = account.AccountPassword,
                AccountRole = account.AccountRole
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UserProfileViewModel model)
        {
            var userId = (short)(HttpContext.Session.GetInt32("UserId") ?? 0);
            if (model.AccountId != userId)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var account = new SystemAccountViewModel
            {
                AccountId = model.AccountId,
                AccountName = model.AccountName,
                AccountEmail = model.AccountEmail,
                AccountPassword = model.AccountPassword,
                AccountRole = model.AccountRole
            };

            var (success, message) = await _apiService.UpdateAccountAsync(model.AccountId, account);
            if (success)
            {
                // Cập nhật lại session
                HttpContext.Session.SetString("UserName", model.AccountName);
                HttpContext.Session.SetString("UserEmail", model.AccountEmail);
                TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return View(model);
        }
    }
}
