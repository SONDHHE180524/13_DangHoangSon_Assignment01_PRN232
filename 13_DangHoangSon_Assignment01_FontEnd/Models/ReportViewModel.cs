using System.ComponentModel.DataAnnotations;

namespace _13_DangHoangSon_Assignment01_FontEnd.Models
{
    public class ReportSearchViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn từ ngày")]
        [Display(Name = "Từ ngày (Start Date)")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today.AddMonths(-1);

        [Required(ErrorMessage = "Vui lòng chọn đến ngày")]
        [Display(Name = "Đến ngày (End Date)")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Today;

        public List<NewsArticleViewModel> Articles { get; set; } = new List<NewsArticleViewModel>();
    }
}
