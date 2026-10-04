using System.ComponentModel.DataAnnotations;

namespace _13_DangHoangSon_Assignment01_BackEnd.DTOs
{
    public class NewsArticleRequestDto
    {
        [Required]
        [StringLength(20)]
        public string NewsArticleId { get; set; } = null!;

        [StringLength(400)]
        public string? NewsTitle { get; set; }

        [Required]
        [StringLength(150)]
        public string Headline { get; set; } = null!;

        [StringLength(4000)]
        public string? NewsContent { get; set; }

        [StringLength(400)]
        public string? NewsSource { get; set; }

        public short? CategoryId { get; set; }

        public bool? NewsStatus { get; set; }

        public short? CreatedById { get; set; }

        public short? UpdatedById { get; set; }

        // Danh sách ID các Tag được chọn gắn kèm với bài viết
        public List<int> TagIds { get; set; } = new List<int>();
    }
}