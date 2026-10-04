namespace _13_DangHoangSon_Assignment01_BackEnd.DTOs
{
    public class CategoryRequestDto
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryDesciption { get; set; } = string.Empty;
        public short? ParentCategoryId { get; set; }
        public bool? IsActive { get; set; }
    }
}
