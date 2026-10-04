namespace _13_DangHoangSon_Assignment01_BackEnd.DTOs
{
    public class LoginResponseDto
    {
        public short AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string AccountEmail { get; set; } = string.Empty;
        public int Role { get; set; } // 0: Admin, 1: Staff, 2: Lecturer
    }
}