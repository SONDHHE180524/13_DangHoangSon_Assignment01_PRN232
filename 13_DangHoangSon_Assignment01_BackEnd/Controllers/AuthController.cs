using _13_DangHoangSon_Assignment01_BackEnd.DTOs;
using _13_DangHoangSon_Assignment01_BackEnd.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace _13_DangHoangSon_Assignment01_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISystemAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;

        public AuthController(ISystemAccountRepository accountRepository, IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Kiểm tra tài khoản mặc định Admin trong appsettings.json
            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];

            if (string.Equals(request.Email, adminEmail, StringComparison.OrdinalIgnoreCase)
                && request.Password == adminPassword)
            {
                return Ok(new LoginResponseDto
                {
                    AccountId = 0,
                    AccountName = "Administrator",
                    AccountEmail = adminEmail!,
                    Role = 0 // 0 quy ước là Admin
                });
            }

            // 2. Kiểm tra tài khoản Staff / Lecturer trong CSDL
            var account = await _accountRepository.GetAccountByEmailAsync(request.Email);
            if (account == null || account.AccountPassword != request.Password)
            {
                return Unauthorized(new { message = "Email hoặc mật khẩu không chính xác!" });
            }

            return Ok(new LoginResponseDto
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName ?? string.Empty,
                AccountEmail = account.AccountEmail ?? string.Empty,
                Role = account.AccountRole ?? 2
            });
        }
    }
}