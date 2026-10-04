using System.ComponentModel.DataAnnotations;

namespace _13_DangHoangSon_Assignment01_FontEnd.Models
{
    public class CategoryViewModel
    {
        public short CategoryId { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100, ErrorMessage = "Tên danh mục không quá 100 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string CategoryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả danh mục không được để trống")]
        [StringLength(250, ErrorMessage = "Mô tả không quá 250 ký tự")]
        [Display(Name = "Mô tả")]
        public string CategoryDesciption { get; set; } = string.Empty;

        [Display(Name = "Danh mục cha")]
        public short? ParentCategoryId { get; set; }

        [Display(Name = "Trạng thái hoạt động")]
        public bool? IsActive { get; set; } = true;

        public CategoryViewModel? ParentCategory { get; set; }
    }
}
