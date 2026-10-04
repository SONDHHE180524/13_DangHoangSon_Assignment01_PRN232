using System.ComponentModel.DataAnnotations;

namespace _13_DangHoangSon_Assignment01_FontEnd.Models
{
    public class NewsArticleViewModel
    {
        [Display(Name = "Mã bài viết")]
        public string NewsArticleId { get; set; } = string.Empty;

        [Display(Name = "Tiêu đề")]
        public string? NewsTitle { get; set; }

        [Display(Name = "Tiêu đề phụ (Headline)")]
        public string Headline { get; set; } = string.Empty;

        [Display(Name = "Ngày tạo")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Nội dung")]
        public string? NewsContent { get; set; }

        [Display(Name = "Nguồn tin")]
        public string? NewsSource { get; set; }

        [Display(Name = "Danh mục")]
        public short? CategoryId { get; set; }

        [Display(Name = "Trạng thái")]
        public bool? NewsStatus { get; set; } = true;

        [Display(Name = "Người tạo")]
        public short? CreatedById { get; set; }

        [Display(Name = "Người sửa")]
        public short? UpdatedById { get; set; }

        [Display(Name = "Ngày sửa")]
        public DateTime? ModifiedDate { get; set; }

        public CategoryViewModel? Category { get; set; }
        public SystemAccountViewModel? CreatedBy { get; set; }
        public List<TagViewModel> Tags { get; set; } = new List<TagViewModel>();
    }

    public class NewsArticleEditViewModel
    {
        [Required(ErrorMessage = "Mã bài viết không được để trống")]
        [StringLength(20, ErrorMessage = "Mã bài viết không quá 20 ký tự")]
        [Display(Name = "Mã bài viết")]
        public string NewsArticleId { get; set; } = string.Empty;

        [StringLength(400, ErrorMessage = "Tiêu đề không quá 400 ký tự")]
        [Display(Name = "Tiêu đề bài viết")]
        public string? NewsTitle { get; set; }

        [Required(ErrorMessage = "Tiêu đề phụ không được để trống")]
        [StringLength(150, ErrorMessage = "Tiêu đề phụ không quá 150 ký tự")]
        [Display(Name = "Tiêu đề phụ (Headline)")]
        public string Headline { get; set; } = string.Empty;

        [StringLength(4000, ErrorMessage = "Nội dung không quá 4000 ký tự")]
        [Display(Name = "Nội dung")]
        public string? NewsContent { get; set; }

        [StringLength(400, ErrorMessage = "Nguồn không quá 400 ký tự")]
        [Display(Name = "Nguồn tin")]
        public string? NewsSource { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public short? CategoryId { get; set; }

        [Display(Name = "Kích hoạt hiển thị (Active)")]
        public bool? NewsStatus { get; set; } = true;

        public short? CreatedById { get; set; }
        public short? UpdatedById { get; set; }

        [Display(Name = "Gắn thẻ (Tags)")]
        public List<int> TagIds { get; set; } = new List<int>();
    }
}
