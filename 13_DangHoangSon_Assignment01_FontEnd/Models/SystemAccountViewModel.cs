using System.ComponentModel.DataAnnotations;

namespace _13_DangHoangSon_Assignment01_FontEnd.Models
{
    public class SystemAccountViewModel
    {
        public short AccountId { get; set; }

        [Required(ErrorMessage = "Tên tài khoản không được để trống")]
        [StringLength(100, ErrorMessage = "Tên tài khoản không được vượt quá 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string AccountName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(70, ErrorMessage = "Email không được vượt quá 70 ký tự")]
        [Display(Name = "Email")]
        public string AccountEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn vai trò")]
        [Display(Name = "Vai trò")]
        public int? AccountRole { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(70, MinimumLength = 1, ErrorMessage = "Mật khẩu không được để trống")]
        [Display(Name = "Mật khẩu")]
        public string AccountPassword { get; set; } = string.Empty;

        public string RoleName => AccountRole switch
        {
            0 => "Admin",
            1 => "Staff",
            2 => "Lecturer",
            _ => "Unknown"
        };
    }

    public class UserProfileViewModel
    {
        public short AccountId { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string AccountName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(70)]
        [Display(Name = "Email")]
        public string AccountEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(70)]
        [Display(Name = "Mật khẩu mới")]
        public string AccountPassword { get; set; } = string.Empty;

        public int? AccountRole { get; set; }
    }
}
