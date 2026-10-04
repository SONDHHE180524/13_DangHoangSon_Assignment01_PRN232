namespace _13_DangHoangSon_Assignment01_BackEnd.DTOs
{
    public class AccountRequestDto
    {
        public short AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? AccountEmail { get; set; }
        public int? AccountRole { get; set; }
        public string? AccountPassword { get; set; }
    }
}
