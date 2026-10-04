using _13_DangHoangSon_Assignment01_FontEnd.Models;
using _13_DangHoangSon_Assignment01_FontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace _13_DangHoangSon_Assignment01_FontEnd.Controllers
{
    public class AuthController : Controller
    {
        private readonly IApiService _apiService;

        public AuthController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (HttpContext.Session.GetInt32("UserId").HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _apiService.LoginAsync(model.Email, model.Password);
            if (result == null)
            {
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác!");
                return View(model);
            }

            // Lưu thông tin đăng nhập vào Session
            HttpContext.Session.SetInt32("UserId", result.AccountId);
            HttpContext.Session.SetString("UserName", result.AccountName);
            HttpContext.Session.SetString("UserEmail", result.AccountEmail);
            HttpContext.Session.SetInt32("UserRole", result.Role);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Điều hướng theo Role
            return result.Role switch
            {
                0 => RedirectToAction("Index", "Accounts"),
                1 => RedirectToAction("Index", "NewsArticles"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
