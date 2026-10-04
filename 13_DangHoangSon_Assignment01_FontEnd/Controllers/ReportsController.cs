using _13_DangHoangSon_Assignment01_FontEnd.Filters;
using _13_DangHoangSon_Assignment01_FontEnd.Models;
using _13_DangHoangSon_Assignment01_FontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace _13_DangHoangSon_Assignment01_FontEnd.Controllers
{
    [CustomAuthorize(0)] // Chỉ Role Admin (0) mới xem được báo cáo thống kê
    public class ReportsController : Controller
    {
        private readonly IApiService _apiService;

        public ReportsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // Báo cáo thống kê tin tức từ StartDate đến EndDate, sắp xếp giảm dần theo ngày tạo
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var s = startDate ?? DateTime.Today.AddMonths(-1);
            var e = endDate ?? DateTime.Today;

            var articles = await _apiService.GetReportAsync(s, e);

            var model = new ReportSearchViewModel
            {
                StartDate = s,
                EndDate = e,
                Articles = articles
            };

            return View(model);
        }
    }
}
